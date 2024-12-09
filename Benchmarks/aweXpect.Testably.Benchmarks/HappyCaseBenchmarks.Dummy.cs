using BenchmarkDotNet.Attributes;

namespace aweXpect.Testably.Benchmarks;

public partial class HappyCaseBenchmarks
{
	private readonly int _value = 2;
	private readonly Dummy _dummy = new();

	[Benchmark]
	public int Dummy_aweXpect()
		=> _dummy.Double(_value);
}
