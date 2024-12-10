using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HaveDirectory
{
	public class WithFilesTests
	{
		[Fact]
		public async Task BeEmpty_WhenDirectoryIsNotEmpty_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

			async Task Act()
				=> await That(sut).Should().HaveDirectory(path).WithFiles(f => f.BeEmpty());

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have directory '{path}' which files should be empty,
				              but files was [foo\bar.txt]
				              """);
		}

		[Fact]
		public async Task BeEmpty_WhenDirectoryIsEmpty_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path);

			async Task Act()
				=> await That(sut).Should().HaveDirectory(path).WithFiles(f => f.BeEmpty());

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task AllHaveContent_WhenContentIsDifferent_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

			async Task Act()
				=> await That(sut).Should().HaveDirectory(path)
					.WithFiles(f => f.HaveAll(x => x.HaveContent("SOME-CONTENT")));

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have directory '{path}' which files should have all items have Content equal to "SOME-CONTENT",
				              but not all were
				              """);
		}

		[Fact]
		public async Task AllHaveContent_WhenContentMatches_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

			async Task Act()
				=> await That(sut).Should().HaveDirectory(path)
					.WithFiles(f => f.HaveAll(x => x.HaveContent("SOME-CONTENT").IgnoringCase()));

			await That(Act).Should().NotThrow();
		}
	}
}
