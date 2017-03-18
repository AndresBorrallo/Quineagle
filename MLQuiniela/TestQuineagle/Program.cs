using log4net.Config;
using System;
using log4net;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using libQuinEagle;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Data.SQLite;
using libQuinEagle.Clasification;
using Newtonsoft.Json;

namespace TestQuineagle
{
    class Program
    {
        /// <summary>
        /// Proyecto para testeo de algoritmos frente a todos los partidos de la temporada 2017 en adelante.
        /// </summary>
        static public ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Log4Net
            InitializeLog4net();

            // Cargamos la configuracion
            Log.Info("Cargando configuracion");
            Configuration configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(@"./Configuration.json"));

            // Cargamos todos los partidos en una lista
            List<Match> matches = _getMatchesStoredDB();

            // Seleccionamos todas las jornadas y años que tenemos partidos
            List<JourneyDateClassification> journeys = matches.Select(a => new JourneyDateClassification(){ journey = a.journey, season = a.season })
                                                            .Distinct(new JourneyDateClassification.DistinctJourneyDateClassificationComparer())
                                                            .ToList();

            // Cogemos todas las clasificaciones y las almacenamos para estas jornadas de estos años
            Dictionary<int, Dictionary<LeagueEnum, LeagueTable>> classifications = _getClassifications(journeys, configuration);
            
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

                    ar.DownloadLeague(LeagueEnum.PRIMERA);
                    ar.DownloadLeague(LeagueEnum.SEGUNDA);

                    LeagueTable firstDiv = ar.GetLeague(LeagueEnum.PRIMERA);
                    LeagueTable secondDiv = ar.GetLeague(LeagueEnum.SEGUNDA);

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
        private static List<Match> _getMatchesStoredDB()
        {
            List<Match> matches = new List<Match>();

            SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
            conexion.Open();

            string consulta = " select t.name, t2.name, m.result, j.number_journey, j.season " +
                              " from t_match m " +
                                    "join t_team t " +
                                        "on (m.id_hometeam = t.id_team) " +
                                    "join t_team t2 " +
                                        "on (m.id_awayteam = t2.id_team) " +
                                    "join t_journey j " +
                                        "on (j.id_journey = m.id_journey)";
            SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
            SQLiteDataReader data = cmd.ExecuteReader();

            Log.Info($"Partidos encontrados a contrastar:");
            Log.Info($"--------------------------------");

            // Leemos los datos de forma repetitiva
            while (data.Read())
            {
                Match m = new Match()
                {
                    homeTeam = Convert.ToString(data[0]),
                    awayTeam = Convert.ToString(data[1]),
                    result = Convert.ToString(data[2]),
                    journey = Convert.ToInt32(data[3]),
                    season = Convert.ToInt32(data[4])   
                };
                Log.Debug($"{m.homeTeam} - {m.awayTeam}: {m.result} (Temporada:{m.season-1}/{m.season} Jornada:{m.journey})");
                matches.Add(m);
            }

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
