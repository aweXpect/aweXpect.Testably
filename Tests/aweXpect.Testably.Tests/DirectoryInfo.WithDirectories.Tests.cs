using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class WithDirectories
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubdirectoriesEmpty_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).WithDirectories(d => d.IsEmpty());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubdirectoriesPresent_AndExpectedEmpty_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo/bar");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");
				string expectedPath = fileSystem.Path.Combine(dirInfo.FullName, "bar");

				async Task Act()
				{
					await That(dirInfo).WithDirectories(d => d.IsEmpty());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					               whose subdirectories are empty,
					              but subdirectories was [
					                {expectedPath}
					              ]
					              """);
			}
		}
	}
}
