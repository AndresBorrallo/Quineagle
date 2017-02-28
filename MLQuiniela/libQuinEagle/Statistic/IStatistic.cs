
namespace libQuinEagle.Statistic
{
	public interface IStatistic
	{
		float Weight { get; set; }

		float GetStatistic (Fixture fixture);
	}
}
