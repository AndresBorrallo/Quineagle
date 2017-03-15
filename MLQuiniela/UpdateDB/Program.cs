using libQuinEagle.Clasification;
using log4net;
using log4net.Config;
using System;
using Classes.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            // Cargamos clasificacion
            Log.Info("Cargando clasificacion");
            ApiRequester ar = new ApiRequester()
            {
                API_KEY = configuration.API_KEY,
                API_URL = configuration.API_URL,
                RequestHeader = configuration.RequestHeader,
                LeagueRequest = configuration.LeagueRequest
            };
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
