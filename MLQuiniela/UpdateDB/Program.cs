using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using libQuinEagle;
using System.Collections.Generic;
using System.Linq;
using libQuinEagle.Fixtures;
using Newtonsoft.Json.Linq;
using SQLite;
using SQLite_Net.Extensions.Readers;

namespace UpdateDB
{
    /// <summary>
    /// Proyecto para la actualización de la BBDD, recogiendo el resultado de los partidos
    /// </summary>
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
            SQLiteConnection conexion = new SQLiteConnection("../../../db/quineagle.db");
            //conexion.Open();
			conexion.BeginTransaction();
            _deleteDB(conexion);
            string consulta = "select number_journey, season from t_journey";
			//SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
			//SQLiteDataReader data = cmd.ExecuteReader();
			var data = conexion.ExecuteReader(consulta);
			List<Journey> journeys = new List<Journey>();

            Log.Info($"Jornadas encontradas a procesar:");
            Log.Info($"--------------------------------");

			// Leemos los datos de forma repetitiva
			//while (data.Read())
			foreach (var readerItem in data)
            {
                int nj =0, sea = 0;

				//if (data[1] != System.DBNull.Value)
				if(readerItem["number_journey"] != System.DBNull.Value )
					nj = Convert.ToInt16(readerItem["number_journey"]);

				//if (data[4] != System.DBNull.Value)
				if( readerItem["season"] != System.DBNull.Value )
					sea = Convert.ToInt16( readerItem["season"] );

                Journey j = new Journey()
                {
                    number_journey = nj,
                    season = sea,
                };

                journeys.Add(j);

                // Y los mostramos                
                Log.Info($"Jornada número: {nj}\tTemporada: {sea}");
            }

            conexion.Close();

            return journeys;
        }

        /// <summary>
        /// Guarda todos los partidos ya jugados de primera y segunda division
        /// </summary>
        /// <param name="j">Jornada a procesar</param>
        private static void _loadAndSaveJourneyResult(Journey j)
        {
            int journey = j.number_journey;
            int season = j.season;

            // Cargamos toda la informacion de quinielista
            FixtureRequester fr = new FixtureRequester() {
                QuinielaFixtureURL = "https://www.quinielista.es/ayudas/module_help_other_upcoming.asp?num_jornada=" + journey + "&num_temporada=" + season
            };

            fr.LoadFixtures();            
            List<Fixture> fixtures = fr.GetFixtures();

            // Cogemos solo los nombres de los equipos y nos quedamos con los partidos utiles (Primera y segunda division ya jugados)
            var teams = Teams.TeamsNames.Select(x => x.Key);
            var utilMatches = fixtures.Where(a => a.Result != QuinielaResult.VOID && teams.Contains(a.HomeTeam) && teams.Contains(a.AwayTeam));

            try
            {
                // Abrimos la conexion
                SQLiteConnection conexion = new SQLiteConnection("../../../db/quineagle.db");
				conexion.BeginTransaction();

                // Construimos la query para insertar
                List<Journey> journeys = new List<Journey>();
                string consulta = "INSERT INTO t_match (id_homeTeam,id_awayTeam,result,id_journey) VALUES ";
                foreach (var f in utilMatches)
                {
                    consulta += "( (SELECT id_team from t_team where name = '" + f.HomeTeam + "'), "
                              + "  (SELECT id_team from t_team where name = '" + f.AwayTeam + "'), "
                              + "'" + _parseResult(f.Result) + "',"
                              + "(SELECT id_journey from t_journey where number_journey = '" + j.number_journey + "' and season = '" + j.season + "')),";
                    Log.Debug($"Jornada {j.number_journey}: {f.HomeTeam} - {f.AwayTeam} : {f.Result}");
                }
                consulta = consulta.TrimEnd(',');

                // Si hay algun partido, lo insertamos
                if (utilMatches.Count() > 0)
                {
                    //SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
					Log.Info($"{conexion.Execute(consulta)} rows inserted");
                }

                // Cerramos la conexion
                conexion.Close();
            }catch(Exception e)
            {
				Log.Error($"ERROR: {e.Message}");
            }
        }

        /// <summary>
        /// Borra toda la información de los partidos que tenemos, dado que vamos a actualizarlo todo
        /// </summary>
        /// <param name="conexion"> Conexion a base de datos</param>
        private static void _deleteDB(SQLiteConnection conexion)
        {
            try
            {
                string consulta = "delete from t_match";
				var data = conexion.Execute(consulta);
                //SQLiteCommand cmd = new SQLiteCommand(consulta, conexion);
                //cmd.ExecuteNonQuery();
            }
            catch (Exception e) {
                Log.Error("ERROR:" + e.Message);
            }

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

        /// <summary>
        /// Carga todos los nombres de equipos con tokens
        /// </summary>
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
