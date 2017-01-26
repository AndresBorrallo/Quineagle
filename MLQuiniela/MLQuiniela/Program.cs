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
			static readonly ILog _log = LogManager.GetLogger( typeof( Program ) );

            
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
