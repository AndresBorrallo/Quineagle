using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using libQuinEagle;
using System.Data.SQLite;
using libQuinEagle.Clasification;
using System.Collections.Generic;
using System.Linq;
using libQuinEagle.Fixtures;
using Newtonsoft.Json.Linq;

namespace UpdateDB
{
    class Program
    {
        // Información de las jornadas sacadas de
        // http://quinielamania.blogspot.com.es/p/calendario-quiniela.html
        

        static public ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static public string cadenaURL = "https://www.quinielista.es/ayudas/module_help_other_upcoming.asp?num_jornada=$JOURNEY$&num_temporada=$SEASON$";

        static void Main(string[] args)
        {
            // Log4Net
            InitializeLog4net();
             
            // Cargamos desde base de datos la configuración de las jornadas
            List<Journey> journeys = _getJourneysFromDB();

            // Cargamos los equipos
            _loadTeams();

            // Recorremos las jornadas obteniendo los resultados
            foreach (Journey j in journeys)
            {
                _loadAndSaveJourneyResult(j);
            } 
                       
            Console.ReadKey();
        }

        private static List<Journey> _getJourneysFromDB()
        {
            SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
            conexion.Open();
            _deleteDB(conexion);
            string consulta = "select * from t_journey";
            SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
            SQLiteDataReader data = cmd.ExecuteReader();
            List<Journey> journeys = new List<Journey>();

            Console.WriteLine($"Jornadas encontradas a procesar:");
            Console.WriteLine($"--------------------------------");

            // Leemos los datos de forma repetitiva
            while (data.Read())
            {
                int nj =0, sea = 0;
                
                if (data[1] != System.DBNull.Value)
                    nj = Convert.ToInt16(data[1]);
                
                if (data[4] != System.DBNull.Value)
                    sea = Convert.ToInt16(data[4]);

                Journey j = new Journey()
                {
                    number_journey = nj,
                    season = sea,
                };

                journeys.Add(j);

                // Y los mostramos                
                Console.Write($"Jornada número: {nj}");

                Console.Write($"\tTemporada: {sea}\n");
            }

            conexion.Close();
            Console.WriteLine();

            return journeys;
        }

        private static void _loadAndSaveJourneyResult(Journey j)
        {
            int journey = j.number_journey;
            int season = j.season;

            FixtureRequester fr = new FixtureRequester() {
                QuinielaFixtureURL = "https://www.quinielista.es/ayudas/module_help_other_upcoming.asp?num_jornada=" + journey + "&num_temporada=" + season
            };

            fr.LoadFixtures();            
            List<Fixture> fixtures = fr.GetFixtures();

            var teams = Teams.TeamsNames.Select(x => x.Key);
            var utilMatches = fixtures.Where(a => a.Result != QuinielaResult.VOID && teams.Contains(a.HomeTeam) && teams.Contains(a.AwayTeam));

            SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
            conexion.Open();

            string consulta = "INSERT INTO `t_match` (id_homeTeam,id_awayTeam,result,id_journey) VALUES ";
            List<Journey> journeys = new List<Journey>();
            foreach (var f in utilMatches)
            {
                consulta += "( (SELECT id_team from `t_team` where name = '" + f.HomeTeam + "'), "
                          + "  (SELECT id_team from `t_team` where name = '" + f.AwayTeam + "'), "
                          + "'" + _parseResult(f.Result) + "',"
                          + "(SELECT id_journey from `t_journey` where number_journey = '" + j.number_journey + "' and season = '" + j.season + "')),";                
                Console.WriteLine($"Jornada {j.number_journey}: {f.HomeTeam} - {f.AwayTeam} : {f.Result}");
            }
            consulta = consulta.TrimEnd(',');
            if (utilMatches.Count() > 0)
            {
                SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
                Console.Write(cmd.ExecuteNonQuery() + " rows inserted");
            }
            conexion.Close();
        }

        private static void _deleteDB(SQLiteConnection conexion)
        {
            string consulta = "delete from t_match";
            SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
            cmd.ExecuteNonQuery();
        }

        private static char _parseResult(QuinielaResult res)
        {
            char r;
            if (res == QuinielaResult.ONE)
                r = '1';
            else if (res == QuinielaResult.X)
                r = 'X';
            else if (res == QuinielaResult.TWO)
                r = '2';
            else
                r = 'E';

            return r;
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
