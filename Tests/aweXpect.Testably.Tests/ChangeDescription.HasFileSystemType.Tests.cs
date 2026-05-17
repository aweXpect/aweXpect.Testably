using Testably.Abstractions.Testing;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasFileSystemType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenItDiffers_ShouldSucceed()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(FileSystemTypes.Directory);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileSystemTypeDiffers_ShouldFail()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(FileSystemTypes.Directory);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has file system type Directory,
					             but it was File
					             """);
			}

			[Fact]
			public async Task WhenFileSystemTypeMatches_ShouldSucceed()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(FileSystemTypes.File);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
