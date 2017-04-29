using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libQuinEagle;
using libQuinEagle.Clasification;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using SQLite_Net.Extensions.Readers;

namespace TestQuineagle
{
	class Program
    {
        /// <summary>
        /// Proyecto para testeo de algoritmos frente a todos los partidos de la temporada 2017 en adelante.
        /// </summary>
        static public ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <param name="args"></param>
        public static void Main(string[] args)
        {
			// Log4Net
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

	        // Cargamos todos los partidos en una lista
	        List<Match> matches = _getMatchesStoredDB(qc);

			// Seleccionamos todas las jornadas y años que tenemos partidos
			//List<JourneyDateClassification> journeys = matches.Select(a => new JourneyDateClassification(){ journey = a.Journey, season = a.season })
			//                                                .Distinct(new JourneyDateClassification.DistinctJourneyDateClassificationComparer())
			//                                                .ToList();

			// Cogemos todas las clasificaciones y las almacenamos para estas jornadas de estos años
			//Dictionary<int, Dictionary<LeagueEnum, LeagueTable>> classifications = _getClassifications(journeys, configuration);

			List<Match> predictions = new List<Match>();
			matches.ForEach(m =>
			{
				Match match = m.Clone();
				QuinielaResult qr = QuinielaResult.VOID;

				match.fixture.Probability = qc.GetResult(m.fixture, out qr);
				match.fixture.Result = qr;
				predictions.Add(match);
			});

			int aciertos = matches.Where(a => predictions.Any(
				b => b.fixture.AwayTeam == a.fixture.AwayTeam &&
				b.fixture.HomeTeam == a.fixture.HomeTeam &&
				b.fixture.Journey == a.fixture.Journey &&
				(b.fixture.Result & a.fixture.Result) != QuinielaResult.VOID)).Count();

			Log.Info($"Aciertos {aciertos} de {predictions.Count}");
			Log.Info($"Total: {(float)aciertos / (float)predictions.Count * 100f} %");

            Console.ReadKey();
        }

        private static Dictionary<int, Dictionary<LeagueEnum, LeagueTable>> _getClassifications(List<JourneyDateClassification> journeys, Configuration configuration)
        {
            // Cargamos clasificacion
            Log.Info("Cargando las clasificaciones");
            Dictionary<int, Dictionary<LeagueEnum, LeagueTable>> classifications = new Dictionary<int, Dictionary<LeagueEnum, LeagueTable>>();
            foreach (var j in journeys)
            {
                if (j.journey == 2) // TODO if (j.journey > 1)
                {
                    Dictionary<string, string> leagueRequest = new Dictionary<string, string>();
                    leagueRequest.Add(EnumUtility.GetDescriptionFromEnumValue(LeagueEnum.PRIMERA), "competitions/436/leagueTable/?matchday=" + (j.journey - 1));
                    leagueRequest.Add(EnumUtility.GetDescriptionFromEnumValue(LeagueEnum.SEGUNDA), "competitions/437/leagueTable/?matchday=" + (j.journey - 1));

					ApiRequester ar = new ApiRequester()
					{
						API_KEY = configuration.API_KEY,
						API_URL = configuration.API_URL,
						RequestHeader = configuration.RequestHeader,
						LeagueRequest = leagueRequest
					};

                    /*
                    ar.DownloadLeague(LeagueEnum.PRIMERA);
                    ar.DownloadLeague(LeagueEnum.SEGUNDA);
					*/

					LeagueTable firstDiv = ar.GetLeague(LeagueEnum.PRIMERA,j.journey);
					LeagueTable secondDiv = ar.GetLeague(LeagueEnum.SEGUNDA,j.journey);

                    Dictionary <LeagueEnum, LeagueTable> table = new Dictionary<LeagueEnum, LeagueTable>();
                    table.Add(LeagueEnum.PRIMERA, firstDiv);
                    table.Add(LeagueEnum.SEGUNDA, secondDiv);

                    classifications.Add(j.journey, table);

                }
            }
            return classifications;
        }

        /// <summary>
        /// Carga todos los partidos de BBDD
        /// </summary>
        /// <returns>Lista de partidos almacenados</returns>
        private static List<Match> _getMatchesStoredDB(QuinEagleCalculator qc)
        {
            List<Match> matches = new List<Match>();

            //SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
			SQLiteConnection conexion = new SQLiteConnection("../../../db/quineagle.db");

			//conexion.Open();
			conexion.BeginTransaction();

			/*
            string consulta = " select t.name, t2.name, m.result, j.number_journey, j.season " +
                              " from t_match m " +
                                    "join t_team t " +
                                        "on (m.id_hometeam = t.id_team) " +
                                    "join t_team t2 " +
                                        "on (m.id_awayteam = t2.id_team) " +
                                    "join t_journey j " +
                                        "on (j.id_journey = m.id_journey)";
			*/
			string consulta = "select t.name as hometeam, t2.name as awayteam, m.result as result, j.number_journey as journey, j.season as season, j.first_div as first_div, j.second_div as second_div" +
							  " from t_match m " +
									"join t_team t " +
										"on (m.id_hometeam = t.id_team) " +
									"join t_team t2 " +
										"on (m.id_awayteam = t2.id_team) " +
									"join t_journey j " +
										"on (j.id_journey = m.id_journey) where j.number_journey > 5";
			
			//SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
			var data = conexion.ExecuteReader(consulta);
            //SQLiteDataReader data = cmd.ExecuteReader();


            Log.Info($"Partidos encontrados a contrastar:");
            Log.Info($"--------------------------------");

            // Leemos los datos de forma repetitiva
            //while (data.Read())
			foreach (var readerItem in data)
            {
				Log.Info(string.Join(";", readerItem.Fields.Select(e => e + ":" + readerItem[e])));

				Match m = new Match()
				{
					fixture = new Fixture()
					{
						HomeTeam = Convert.ToString(readerItem["hometeam"]),
						AwayTeam = Convert.ToString(readerItem["awayteam"]),
						Result = EnumUtility.GetEnumValueFromDescription<QuinielaResult>(Convert.ToString(readerItem["result"]))
					},
					journey = new Journey() 
					{
						first_div = Convert.ToInt32(readerItem["first_div"]),
						second_div = Convert.ToInt32(readerItem["second_div"]),
						id_journey = Convert.ToInt32(readerItem["id_journey"]),
						number_journey = Convert.ToInt32(readerItem["number_journey"]),
						season = Convert.ToInt32(readerItem["season"])
					}
                };
				// Ahora tenemos que averiguar si es de primera o de segunda
				if (qc.GetLeague(m.fixture.HomeTeam) == LeagueEnum.PRIMERA)
					m.fixture.Journey = m.journey.first_div;
				else
					m.fixture.Journey = m.journey.second_div;
				
				Log.Debug($"{m.fixture.HomeTeam} - {m.fixture.AwayTeam}: {EnumUtility.GetDescriptionFromEnumValue(m.fixture.Result)} (Temporada:{m.journey.season - 1}/{m.journey.season} Jornada:{m.fixture.Journey})");
                matches.Add(m);   
            }

			conexion.Close();

            return matches;
        }
        
        private static void _loadTeams()
        {
            // Cargamos nombres de equipos
            Log.Info("Cargando nombres de equipos de 'TeamsNames.json'");
            var jObject = JObject.Parse(File.ReadAllText(@"./TeamsNames.json"));
            var jToken = jObject.GetValue("TeamsNames");
            Teams.TeamsNames = (Dictionary<string, List<string>>)jToken.ToObject(typeof(Dictionary<string, List<string>>));
            Log.Info($"Se han cargado {Teams.TeamsNames.Values.Sum(x => x.Count) } nombres de equipos");
        }


        private static void InitializeLog4net()
        {
            Assembly EntryAssembly = Assembly.GetEntryAssembly();
            string AppPath = string.Format("{0}{1}", Path.GetDirectoryName(EntryAssembly.Location), Path.DirectorySeparatorChar);
            Directory.SetCurrentDirectory(AppPath);
            XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));   // configure log4net
            Log.Info(AppPath + ", " + Environment.CurrentDirectory);
            Log.Info("Initializating application");
        }
    }
}
