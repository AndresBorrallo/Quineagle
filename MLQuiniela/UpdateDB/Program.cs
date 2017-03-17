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

namespace UpdateDB
{
    class Program
    {
        static public ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            InitializeLog4net();

            // Cargamos la configuracion
            Log.Info("Cargando configuracion");
            Configuration configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(@"./Configuration.json"));

            // Cargamos desde base de datos la configuración de las jornadas
            List<Journey> journeys = _getJourneysFromDB();

            // Para cada jornada, cargamos la clasificación y los resultados

            foreach(Journey j in journeys){
                _loadFirstDiv();
                _loadSecondDiv();
                _processJourney();
            };

            _insert2DB();
                        
            Console.ReadKey();
        }

        private static List<Journey> _getJourneysFromDB()
        {
            SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
            conexion.Open();

            string consulta = "select * from t_journey";
            SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
            SQLiteDataReader data = cmd.ExecuteReader();
            List<Journey> journeys = new List<Journey>();

            Console.WriteLine($"Jornadas encontradas a procesar:");
            Console.WriteLine($"--------------------------------\n");

            // Leemos los datos de forma repetitiva
            while (data.Read())
            {
                int? nj = null, fd = null, sd = null;
                
                if (data[1] != System.DBNull.Value)
                    nj = Convert.ToInt16(data[1]);

                if (data[2] != System.DBNull.Value)
                    fd = Convert.ToInt16(data[2]);

                if (data[3] != System.DBNull.Value)
                    sd = Convert.ToInt16(data[3]);

                Journey j = new Journey()
                {
                    number_journey = nj,
                    first_div = fd,
                    second_div = sd
                };

                journeys.Add(j);

                // Y los mostramos                
                Console.WriteLine($"Jornada número: {nj}, Primera División: {fd}, Segunda División: {sd}");
            }

            return journeys;
        }

        private static void _loadFirstDiv()
        {
            /*ApiRequester ar = new ApiRequester()
            {
                API_KEY = configuration.API_KEY,
                API_URL = configuration.API_URL,
                RequestHeader = configuration.RequestHeader,
                LeagueRequest = configuration.LeagueRequest
            };*/
        }

        private static void _loadSecondDiv()
        {

        }

        private static void _processJourney()
        {

        }

        private static void _insert2DB() { }

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
