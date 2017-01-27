using System;
using MLQuiniela.Historic;

namespace MLQuiniela.Statistic
{
	public class HistoricalStatistic : IStatistic
	{
		public HistoricMatchs historics { get; set; }

		public float Weight { get; set; }

		public float GetStatistic( Fixture empairment )
		{
			var statistic = historics.GetStatistic( empairment.HomeTeam, empairment.AwayTeam );

			// OJO CUIDADO!! quizas haya que devolver otra cosa o hacer calculos con los resultados
			return statistic.WinsPercent;
		}
	}
}
