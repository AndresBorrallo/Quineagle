using System;
using System.Collections.Generic;

namespace libQuinEagle.Clasification
{
	public class Self
	{
		public string href { get; set; }
	}

	public class Competition
	{
		public string href { get; set; }
	}

	public class Links
	{
		public Self self { get; set; }
		public Competition competition { get; set; }
	}

	public class Team
	{
		public string href { get; set; }
	}

	public class Links2
	{
		public Team team { get; set; }
	}

	public class Summary
	{
		public int goals { get; set; }
		public int goalsAgainst { get; set; }
		public int wins { get; set; }
		public int draws { get; set; }
		public int losses { get; set; }
	}

	public class Standing
	{
		public Links2 _links { get; set; }
		public int position { get; set; }
		public string teamName { get; set; }
		public string crestURI { get; set; }
		public int playedGames { get; set; }
		public int points { get; set; }
		public int goals { get; set; }
		public int goalsAgainst { get; set; }
		public int goalDifference { get; set; }
		public int wins { get; set; }
		public int draws { get; set; }
		public int losses { get; set; }
		public Summary home { get; set; }
		public Summary away { get; set; }
	}

	/// <summary>
	/// Representa una clasificacion de Liga (json)
	/// </summary>
	public class LeagueTable
	{
		public Links _links { get; set; }
		public string leagueCaption { get; set; }
		public int matchday { get; set; }
		public IList<Standing> standing { get; set; }
	}
}
