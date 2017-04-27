using System.Linq;
using System.Reflection;
using log4net;
using libQuinEagle.Clasification;

namespace libQuinEagle.Statistic
{
	public class ClassificationStatistic : IStatistic
	{
		private ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		public ApiRequester req { get; set; }
		
		public float Weight { get; set; }


		/// <summary>
		/// Resuelve la ecuacion y = (x + 100) / 2 siendo X la diferencia de % de la posicion en la tabla
		/// </summary>
		/// <returns>The statistic.</returns>
		/// <param name="fixture">Fixture.</param>
		public float GetStatistic( Fixture fixture )
		{
			float res = 0f;
			LeagueEnum liga = req.GetDivision( fixture.HomeTeam );

			LeagueTable tabla = req.GetLeague(liga, fixture.Journey);

			if (tabla != null)
			{
				// primero necesitamos saber en que liga está este partido;
				if( tabla.standing.Any( a => a.teamName == fixture.HomeTeam ) )
					liga = LeagueEnum.PRIMERA;
				else
					liga = LeagueEnum.SEGUNDA;

				int puntos_posibles = (tabla.matchday - 1) * 3;

				var home_team = tabla.standing.Where(a => a.teamName == fixture.HomeTeam).FirstOrDefault();
				var away_team = tabla.standing.Where(a => a.teamName == fixture.AwayTeam).FirstOrDefault();

				if (home_team != null && away_team != null)
				{
					float pos_homeTeam = (float)home_team.points / puntos_posibles * 100;
					float pos_awayTeam = (float)away_team.points / puntos_posibles * 100;

					float diferencia = pos_homeTeam - pos_awayTeam;

					Log.Debug($"Posicion de {home_team.teamName} = {pos_homeTeam} || {home_team.points} / {puntos_posibles}");
					Log.Debug($"Posicion de {away_team.teamName} = {pos_awayTeam} || {away_team.points} / {puntos_posibles}");

					res = (diferencia + 100) / 2;

					Log.Debug($"Solucion de {home_team.teamName} vs {away_team.teamName} = {res}");
				}
				else
					Log.Warn($"No se encuentran los datos de clasificacion de {fixture.HomeTeam} o de {fixture.AwayTeam}");

			}
			else
				res = -1f;
			
			return res;
		}
	}
}
