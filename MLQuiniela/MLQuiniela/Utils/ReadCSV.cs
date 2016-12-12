using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLQuiniela.Classes;
using System.Dynamic;

namespace MLQuiniela.Utils
{
    class ReadCSV : DynamicObject
    {
        // Store the path to the directory with results.
        private static string baseDirectory = "..\\..\\..\\..\\Historic Results\\";
        
        public static List<HistoricMatch> readFiles()
        {
            List<HistoricMatch> hMatches = new List<HistoricMatch>();

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(baseDirectory);
            foreach (string fileName in fileEntries)
            {
                Console.WriteLine("File {0} found.", fileName);

                StreamReader reader = new StreamReader(File.OpenRead(fileName));

                // Header
                string line = reader.ReadLine();
               
                while (!reader.EndOfStream)
                {
                    HistoricMatch hMatch = new HistoricMatch();
                    line = reader.ReadLine();
                    string[] values = line.Split(',');

                    hMatches.Add(hMatch.historicMatch(values[0].Trim().ToUpper(),
                            values[1].Trim().ToUpper(), 
                            values[2].Trim().ToUpper(), 
                            values[3].Trim().ToUpper(),
                            values[4].Trim().ToUpper(), 
                            values[5].Trim().ToUpper(), 
                            values[6].Trim().ToUpper()));                    
                }
                reader.Close();
            }

            return hMatches;
        }
    }
}