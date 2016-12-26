using MLQuiniela.Classes;
using System;
using System.Collections.Generic;
using MLQuiniela.Classes;
using MLQuiniela.Statistic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLQuiniela
{
    class Program
    {
        static void Main(string[] args)
        {
            // Historic League Data Load.
            Console.WriteLine("Retrieving Historic Data...");
            List<HistoricMatch> historics = Utils.ReadCSV.readFiles();

            // Week Matchs Load.
            Console.WriteLine("Loading matchs of the week...");

            // Filter util data.
            Console.WriteLine("Filtering util historics...");

            // Process data.
            Console.WriteLine("Processing data...");

            // Last 5 matches for each team
            Console.WriteLine("Searching last matches...");

            // Make posible results.
            Console.WriteLine("Executing algorithm for predictions...");

            // Assing value for results.
            Console.WriteLine("Results will be assigned now.");

            // Print predictions.
            Console.WriteLine("\n------------------------------");
            Console.WriteLine("\tPredictions");
            Console.WriteLine("------------------------------");

			Quiniela quiniela = new Quiniela();
			// add empairments

			IStatistic historical_st = new HistoricalStatistic();
			IStatistic classification_st = new ClassificationStatistic();

			foreach( var a in quiniela.Empairments )
			{
				List<Nomio> formula = new List<Nomio>();

				formula.Add( new Nomio() { Variable = historical_st.GetStatistic( a ), Weight = 0.4f } );
				formula.Add( new Nomio() { Variable = classification_st.GetStatistic( a ), Weight = 0.6f } );

				float solution = formula.Sum( n => n.Variable * n.Weight );
				Console.WriteLine( $"Solution for {a.teamA} vs {a.teamB} = {solution}" );

			}

            Console.ReadKey();
        }

    }
}
