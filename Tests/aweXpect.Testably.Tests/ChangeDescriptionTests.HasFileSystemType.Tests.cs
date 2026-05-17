using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasFileSystemType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenItDiffers_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(FileSystemTypes.Directory);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected");
			}

			[Fact]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenFileSystemTypeDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

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
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(FileSystemTypes.File);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
