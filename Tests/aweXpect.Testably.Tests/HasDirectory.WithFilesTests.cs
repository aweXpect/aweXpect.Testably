using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasDirectory
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
				=> await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected sut to
				              have directory '{path}' which files should be empty,
				              but files was [
				                foo{Path.DirectorySeparatorChar}bar.txt
				              ]
				              """);
		}

		[Fact]
		public async Task BeEmpty_WhenDirectoryIsEmpty_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path);

			async Task Act()
				=> await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task AllHaveContent_WhenContentIsDifferent_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

			async Task Act()
				=> await That(sut).HasDirectory(path)
					.WithFiles(f => f.All().Are(x => x.HasContent("SOME-CONTENT")));

			await That(Act).ThrowsException()
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
				=> await That(sut).HasDirectory(path)
					.WithFiles(f => f.All().Are(x => x.HasContent("SOME-CONTENT").IgnoringCase()));

			await That(Act).DoesNotThrow();
		}
	}
}
