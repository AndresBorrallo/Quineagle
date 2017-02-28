using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libQuinEagle;
using libQuinEagle.Clasification;
using libQuinEagle.Fixtures;
using libQuinEagle.Historic;
using libQuinEagle.Statistic;
using libQuinEagle.Fuzzy;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MLQuiniela
{
    public class Program
    {
		static public ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

        static void Main(string[] args)
        {
			InitializeLog4net();

			// Cargamos la configuracion
			Log.Info( "Cargando configuracion" );
			Configuration configuration = JsonConvert.DeserializeObject<Configuration>( File.ReadAllText( @"./Configuration.json" ) );

			// Cargamos nombres de equipos
			Log.Info( "Cargando nombres de equipos de 'TeamsNames.json'" );
			var jObject = JObject.Parse( File.ReadAllText( @"./TeamsNames.json" ) );
			var jToken = jObject.GetValue( "TeamsNames" );
			Teams.TeamsNames = ( Dictionary<string, List<string>> )jToken.ToObject( typeof( Dictionary<string, List<string>> ) );
			Log.Info( $"Se han cargado {Teams.TeamsNames.Values.Sum( x => x.Count ) } nombres de equipos" );

			// Cargamos los emparejamientos
			Log.Info( "Cargando Emparejamientos" );
			FixtureRequester fr = new FixtureRequester()
			{
				QuinielaFixtureURL = configuration.QuinielaFixtureURL
			};
			fr.LoadFixtures();
			fr.PrintFixtures();

			// Cargamos historicos
			Log.Info( "Cargando historicos" );
			HistoricMatchs hm = new HistoricMatchs();
			hm.LoadHistoric( configuration.Csv_URLs );

			// Cargamos clasificacion
			Log.Info( "Cargando clasificacion" );
			ApiRequester ar = new ApiRequester()
			{
				API_KEY = configuration.API_KEY,
				API_URL = configuration.API_URL,
				RequestHeader = configuration.RequestHeader,
				LeagueRequest = configuration.LeagueRequest
			};
			ar.DownloadLeague( LeagueEnum.PRIMERA );
			ar.DownloadLeague( LeagueEnum.SEGUNDA );

			ar.PrintLeague( LeagueEnum.PRIMERA );
			ar.PrintLeague( LeagueEnum.SEGUNDA );

			// Preparamos clases para hacer calculos
			IStatistic historical_st = new HistoricalStatistic()
			{
				historics = hm,
				Weight = configuration.HistoricWeight
			};


			IStatistic classification_st = new ClassificationStatistic()
			{
				req = ar,
				Weight = configuration.ClasificationWeight
			};

			// Recorremos los emparejamientos y hacemos calculos
			//List<string> Soluciones = new List<string>();
			List<Fixture> fixtures = fr.GetFixtures();
			foreach( var a in fixtures )
			{
				List<Nomio> formula = new List<Nomio>();

				var historical_value = historical_st.GetStatistic( a );

				formula.Add( new Nomio() { Variable = historical_value, Weight = historical_st.Weight } );
				formula.Add( new Nomio() { Variable = classification_st.GetStatistic( a ), Weight = historical_value == 0f ? 1f : classification_st.Weight } );

				float solution = formula.Sum( n => n.Variable * n.Weight );

				a.Probability = solution;
			}

			// Preparamos el Motor de logica difusa
			FuzzyCalculator Fuzzy = new FuzzyCalculator() { MaxMultipleBets = configuration.FuzzyConf.MaxDoubles };

			// Antes de meterle los valores de referecia de las curvas, vemos si hay que bascularla a un lado u otro
			// Obtenemos el centro entre X2_X y X3_X (normalmente sera 50)
			float centro = ( configuration.FuzzyConf.X2_X + configuration.FuzzyConf.X3_X ) / 2f;
			// obtenemos la variacion real con respecto al centro
			float diferencia = fixtures.Select( a => a.Probability ).Average() - centro;
			// ahora desplazamos las funciones
			float X1_1 = configuration.FuzzyConf.X1_1 + diferencia;
			float X2_1 = configuration.FuzzyConf.X2_1 + diferencia;
			float X1_X = configuration.FuzzyConf.X1_X + diferencia;
			float X2_X = configuration.FuzzyConf.X2_X + diferencia;
			float X3_X = configuration.FuzzyConf.X3_X + diferencia;
			float X4_X = configuration.FuzzyConf.X4_X + diferencia;
			float X1_2 = configuration.FuzzyConf.X1_2 + diferencia;
			float X2_2 = configuration.FuzzyConf.X2_2 + diferencia;

			Fuzzy.SetFuzzyValues( X1_1, X2_1, X1_X, X2_X, X3_X, X4_X, X1_2, X2_2 );

			Fuzzy.GetBet( ref fixtures );

			foreach( var f in fixtures )
				Log.Info( f.ToString());

			Log.Info( $"Media calculada = {fixtures.Select( a => a.Probability ).Average()}%" );

			Console.ReadKey();
        }

		private static void InitializeLog4net()
		{
			Assembly EntryAssembly = Assembly.GetEntryAssembly();
			string AppPath = string.Format( "{0}{1}", Path.GetDirectoryName( EntryAssembly.Location ), Path.DirectorySeparatorChar );
			Directory.SetCurrentDirectory( AppPath );
			XmlConfigurator.Configure( new FileInfo( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile ) );   // configure log4net
			Log.Info( AppPath + ", " + Environment.CurrentDirectory );
			Log.Info( "Initializating application" );
		}
    }
}
