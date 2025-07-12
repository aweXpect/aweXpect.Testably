using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenContentDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar2");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act() => await That(fileInfo).HasContent("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             has Content equal to "bar",
					             but it was "bar2" with a length of 4 which is longer than the expected length of 3 and has superfluous:
					               "2"

					             File-Content:
					             bar2
					             """);
			}

			[Fact]
			public async Task WhenContentMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act() => await That(fileInfo).HasContent("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act() => await That(fileInfo).HasContent("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             has Content equal to "bar",
					             but it did not exist
					             """);
			}
		}
	}
}
