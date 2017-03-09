using System;

namespace libQuinEagle.Historic
{
	/// <summary>
	/// Estadisticas historicas par dos equipos
	/// </summary>
	public class MatchStatistic
	{
		public string HomeTeam { get; set; }

		public string AwayTeam { get; set; }

		public int NMatchs { get; set; }

		public float WinsPercent { get; set; }

		public float DrawsPercent { get; set; }

		public float LostPercent { get; set; }

		public int GoalsInFavour { get; set; }

		public int GoalsAgainst { get; set; }

        public int NPoints { get; set; }

        public int HomeNPoints { get; set; }

        public int AwayNPoints { get; set; }

		public int MatchWins { get; set; }

		public int MatchDraws { get; set; }

		public int MatchLosts { get; set; }
	}
}
