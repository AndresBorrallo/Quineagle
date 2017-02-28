using System;
using System.Reflection;
using libQuinEagle.Historic;
using log4net;

namespace libQuinEagle.Statistic
{
	public class HistoricalStatistic : IStatistic
	{
		private ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		public HistoricMatchs historics { get; set; }

		public float Weight { get; set; }

		public float GetStatistic( Fixture fixture )
		{
			float res = 0f;

			var statistic = historics.GetStatistic( fixture.HomeTeam, fixture.AwayTeam );

			Log.Debug( $"{fixture.HomeTeam} vs {fixture.AwayTeam}: P.J.: {statistic.NMatchs} || P.G.: {statistic.WinsPercent}% || P.E.:{statistic.DrawsPercent}% || P.P.: {statistic.LostPercent}%" );

			// OJO CUIDADO!! quizas haya que devolver otra cosa o hacer calculos con los resultados
			if( statistic.NMatchs >= 5 )
			{
				res = statistic.WinsPercent;
			}
			else
				Log.Warn( $"Menos de 5 partidos de historico para {fixture.HomeTeam} vs {fixture.AwayTeam}, no se tendra en cuenta el historico" );

			return res;
		}
	}
}
