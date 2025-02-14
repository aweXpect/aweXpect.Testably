using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasDirectory
{
	public class WithDirectoriesTests
	{
		[Fact]
		public async Task WhenItemCountDiffers_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithSubdirectory("directory1")
				.WithSubdirectory("directory2"));

			async Task Act()
				=> await That(sut).HasDirectory(path).WithDirectories(f => f.HasCount().EqualTo(3));

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected that sut
				              has directory '{path}' whose subdirectories has exactly 3 items,
				              but found only 2
				              """);
		}

		[Fact]
		public async Task WhenItemCountMatches_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithSubdirectory("directory1")
				.WithSubdirectory("directory2"));

			async Task Act()
				=> await That(sut).HasDirectory(path).WithDirectories(f => f.HasCount().EqualTo(2));

			await That(Act).DoesNotThrow();
		}
	}
}
