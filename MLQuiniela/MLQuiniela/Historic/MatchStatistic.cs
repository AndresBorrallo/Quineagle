using System;

namespace MLQuiniela.Historic
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
	}
}
