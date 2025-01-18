using System.IO.Abstractions;
using BenchmarkDotNet.Attributes;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Benchmarks;

public partial class HappyCaseBenchmarks
{
	private readonly string _content = "some-content";
	private readonly IFileSystem _fileSystem = new MockFileSystem();
	private readonly string _path = "foo.txt";

	public HappyCaseBenchmarks()
	{
		_fileSystem.File.WriteAllText(_path, _content);
	}

	[Benchmark]
	public async Task<IFileSystem> Dummy_aweXpect()
		=> await Expect.That(_fileSystem).HasFile(_path).WithContent(_content);
}
