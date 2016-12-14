using System;
using MLQuiniela.Classes;

namespace MLQuiniela.Statistic
{
	public class ClassificationStatistic : IStatistic
	{
		public ClassificationStatistic( /* Add Classification Table here */ )
		{
		}

		/// <summary>
		/// Get Probability of victory of TeamA over TeamB.
		/// Get Points of first Table, that is 100%
		/// Get Points of last Table, that is 0%
		/// so, we do line ecuation:
		/// y = ((y_2 - y_1) / (x_2 - x_1)) * (x - x_1) + y_1
		/// the result is %A - %B -- JAIME, NO LO TENGO MUY CLARO, REMATA LA FORMULA
		/// </summary>
		/// <returns>The statistic.</returns>
		/// <param name="empairment">Empairment.</param>
		public float GetStatistic( Empairment empairment )
		{
			throw new NotImplementedException();
		}
	}
}
