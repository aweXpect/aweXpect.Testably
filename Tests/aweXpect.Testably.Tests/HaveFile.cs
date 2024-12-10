namespace aweXpect.Testably.Tests;

public partial class HaveFile
{
	/// <summary>
	///     Use a fixed random time in each test run to ensure, that the tests don't rely on special times.
	/// </summary>
	private static readonly Lazy<DateTime> CurrentTimeLazy = new(
		() => DateTime.Today.ToUniversalTime().AddSeconds(new Random().Next(-10000000, 10000000)));

	private static DateTime CurrentTime()
		=> CurrentTimeLazy.Value;
}
