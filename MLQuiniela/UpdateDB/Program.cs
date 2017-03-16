using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using libQuinEagle;
using System.Data.SQLite;
using libQuinEagle.Clasification;

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
            _getJourneysFromDB();

            // Cargamos clasificacion
            /*Log.Info("Cargando clasificacion");
            ApiRequester ar = new ApiRequester()
            {
                API_KEY = configuration.API_KEY,
                API_URL = configuration.API_URL,
                RequestHeader = configuration.RequestHeader,
                LeagueRequest = configuration.LeagueRequest
            };*/
            Console.ReadKey();
        }

        private static void _getJourneysFromDB()
        {
            SQLiteConnection conexion = new SQLiteConnection("Data Source=../../../db/quineagle.db;Version=3;New=True;Compress=True;");
            conexion.Open();

            string consulta = "select * from t_journey";
            SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
            SQLiteDataReader data = cmd.ExecuteReader();

            // Leemos los datos de forma repetitiva
            while (data.Read())
            {
                int number_journey = 0, first_div = 0, second_div = 0;

                if (!(data[1].GetType().Name == "DBNull"))
                    number_journey = Convert.ToInt16(data[1]);
                if (!(data[2].GetType().Name == "DBNull"))
                    first_div = Convert.ToInt16(data[2]);
                if (!(data[3].GetType().Name == "DBNull"))
                    second_div = Convert.ToInt16(data[3]);

                // Y los mostramos
                Console.WriteLine($"Jornada número: {number_journey}, Primera División: {first_div}, Segunda División: {second_div}");
            }
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
