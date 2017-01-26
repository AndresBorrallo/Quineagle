using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test_Csv
{
	class MainClass
	{
		public static void Main( string[] args )
		{
			Console.WriteLine( "Hello World!" );

			// read file into a string and deserialize JSON to a type
			Configuration configuration = JsonConvert.DeserializeObject<Configuration>( File.ReadAllText( @"Configuration.json" ) );

			/*
			 HistoricMatchs hm = new HistoricMatchs();

			hm.LoadHistoric( configuration.Csv_URLs );

			hm.GetStatistic( "Sevilla", "Betis" );
			hm.GetStatistic( "Betis", "Sevilla" );
			hm.GetStatistic( "Barcelona B", "Sevilla" );
			*/

			/*
			ApiRequester ar = new ApiRequester()
			{
				API_KEY = configuration.API_KEY,
				API_URL = configuration.API_URL,
				RequestHeader = configuration.RequestHeader,
				LeagueRequest = configuration.LeagueRequest
			};

			ar.GetLeagueTable( LeagueEnum.SEGUNDA );
			*/

			var fq = new FixtureRequester() { QuinielaFixtureURL = configuration.QuinielaFixtureURL };

			fq.LoadFixtures();

			Console.ReadKey();
		}

	}
}
