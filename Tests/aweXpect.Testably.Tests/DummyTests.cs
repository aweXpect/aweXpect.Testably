namespace aweXpect.Testably.Tests;

public class DummyTests
{
	[Fact]
	public async Task DummyTest()
	{
		Dummy sut = new();
		int result = sut.Double(3);

		await That(result).Should().Be(6);
	}
}
