
namespace MLQuiniela.Statistic
{
	public interface IStatistic
	{
		float Weight { get; set; }

		float GetStatistic (Fixture fixture);
	}
}
