using System;
namespace Test_Csv
{
	public class ClasificationElement
	{
		/*
		 {
            "rank": 1,
            "team": "ManCity",
            "teamId": 65,
            "playedGames": 10,
            "crestURI": "http://upload.wikimedia.org/wikipedia/de/f/fd/ManCity.svg",
            "points": 22,
            "goals": 24,
            "goalsAgainst": 8,
            "goalDifference": 16
        },
		 */

		public int rank { get; set; }

		public string team { get; set; }

		public int teamId { get; set; }

		public int playedGames { get; set; }

		public string crestURI
	}
}
