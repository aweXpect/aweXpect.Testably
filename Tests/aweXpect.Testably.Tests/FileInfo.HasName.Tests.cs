using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				IFileSystem fileSystem = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasName("bar.txt");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has name equal to "bar.txt",
					             but it was "foo.txt" which differs at index 0:
					                ↓ (actual)
					               "foo.txt"
					               "bar.txt"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				IFileSystem fileSystem = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasName("foo.txt");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
