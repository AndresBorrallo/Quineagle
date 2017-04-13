using libQuinEagle;

namespace TestQuineagle
{
	public class Match
	{
		public Fixture fixture { get; set; }
		public Journey journey { get; set; }

		public Match Clone()
		{
			Match c = new Match()
			{
				fixture = new Fixture()
				{
					HomeTeam = fixture.HomeTeam,
					AwayTeam = fixture.AwayTeam,
					Journey = fixture.Journey,
					Probability = fixture.Probability,
					Result = fixture.Result
				},
				journey = new Journey()
				{
					first_div = journey.first_div,
					second_div = journey.second_div,
					number_journey = journey.number_journey,
					season = journey.season,
					id_journey = journey.id_journey
				}
			};

			return c;
		}
    }
}
