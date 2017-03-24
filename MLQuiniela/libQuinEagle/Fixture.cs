
namespace libQuinEagle
{
	/// <summary>
	/// Relaciona dos equipos y su resultado
	/// </summary>
    public class Fixture
	{
		public string HomeTeam { get; set; }

		public string AwayTeam { get; set; }

		public QuinielaResult Result { get; set; }

		public float Probability { get; set; }

		public int Journey { get; set; }

		public override string ToString()
		{
			return $"{HomeTeam} - {AwayTeam} - {Probability.ToString("0.00")}% - {EnumUtility.GetDescriptionFromEnumValue(Result)}";
		}
	}
}
