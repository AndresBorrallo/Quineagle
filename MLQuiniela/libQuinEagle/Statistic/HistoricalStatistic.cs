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
				//res = statistic.WinsPercent + (statistic.DrawsPercent / 2f);
			//	res = (statistic.HomeNPoints / statistic.NPoints) * 100f;
				res = statistic.WinsPercent;
			}
			else
			{
				Log.Warn( $"Menos de 5 partidos de historico para {fixture.HomeTeam} vs {fixture.AwayTeam}, no se tendra en cuenta el historico\nSolicitando estadisticas totales de {fixture.HomeTeam}" );
				// Buscamos todos los partidos de ese equipo
				var home_stats = historics.GetStatistic( fixture.HomeTeam, null );
				var away_stats = historics.GetStatistic( null, fixture.AwayTeam );

				if( home_stats.NMatchs >= 5  && away_stats.NMatchs >= 5)
				{
					Log.Debug( $"Estadisticas totales de {fixture.HomeTeam}: P.J.: {home_stats.NMatchs} || P.G.: {home_stats.WinsPercent}% || P.E.:{home_stats.DrawsPercent}% || P.P.: {home_stats.LostPercent}%" );
					Log.Debug( $"Estadisticas totales de {fixture.AwayTeam}: P.J.: {away_stats.NMatchs} || P.G.: {away_stats.WinsPercent}% || P.E.:{away_stats.DrawsPercent}% || P.P.: {away_stats.LostPercent}%" );

					//res = ( home_stats.WinsPercent + ( 100 - away_stats.LostPercent - away_stats.DrawsPercent ) ) / 2;
					res = home_stats.WinsPercent;
					//Log.Debug( $"formula: ( {home_stats.WinsPercent} + (100 - {away_stats.LostPercent} - {away_stats.DrawsPercent} )) / 2 = {res}" );
				}
			}
			return res;
		}
	}
}
