﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libQuinEagle;
using libQuinEagle.Clasification;
using libQuinEagle.Fixtures;
using libQuinEagle.Historic;
using libQuinEagle.Statistic;
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
			List<string> Soluciones = new List<string>();
			foreach( var a in fr.GetFixtures() )
			{
				List<Nomio> formula = new List<Nomio>();

				formula.Add( new Nomio() { Variable = historical_st.GetStatistic( a ), Weight = historical_st.Weight } );
				formula.Add( new Nomio() { Variable = classification_st.GetStatistic( a ), Weight = classification_st.Weight } );

				float solution = formula.Sum( n => n.Variable * n.Weight );
                char forecast = (solution >= 60) ? '1' : ((solution >= 40) ? 'X' : '2');

				Soluciones.Add( $"Solution for {a.HomeTeam}-{a.AwayTeam}: {forecast} \t\tResult :{solution}%" );
			}

			foreach( var s in Soluciones )
				Log.Info( s );

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
