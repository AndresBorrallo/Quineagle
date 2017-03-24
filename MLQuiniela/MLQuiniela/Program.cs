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
			Log.Info("Cargando configuracion");
			Configuration configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(@"./Configuration.json"));

			// Cargamos nombres de equipos
			Log.Info("Cargando nombres de equipos de 'TeamsNames.json'");
			var jObject = JObject.Parse(File.ReadAllText(@"./TeamsNames.json"));
			var jToken = jObject.GetValue("TeamsNames");
			Teams.TeamsNames = (Dictionary<string, List<string>>)jToken.ToObject(typeof(Dictionary<string, List<string>>));
			Log.Info($"Se han cargado {Teams.TeamsNames.Values.Sum(x => x.Count) } nombres de equipos");

			QuinEagleCalculator qc = new QuinEagleCalculator() { configuration = configuration };
			qc.Configure();

			// Cargamos los emparejamientos
			Log.Info("Cargando Emparejamientos");

			FixtureRequester fr = new FixtureRequester() { QuinielaFixtureURL = configuration.QuinielaFixtureURL };
			fr.LoadFixtures();
			var fixtures = fr.GetFixtures();
			fr.PrintFixtures();

			List<Fixture> predictions = new List<Fixture>();
			fixtures.ForEach( a => predictions.Add(qc.GetResult(a)));

			predictions.ForEach(a => Log.Info(a.ToString()));

			Log.Info($"Media calculada = {predictions.Select( a => a.Probability ).Average()}%" );

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
