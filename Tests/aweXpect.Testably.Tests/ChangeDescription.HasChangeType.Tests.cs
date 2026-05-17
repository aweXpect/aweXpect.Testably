using System.IO;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasChangeType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveChangeType_WhenChangeTypeDiffers_ShouldSucceed()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenChangeTypeDiffers_ShouldFail()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(WatcherChangeTypes.Deleted);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has change type Deleted,
					             but it was Created
					             """);
			}

			[Fact]
			public async Task WhenChangeTypeMatches_ShouldSucceed()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(WatcherChangeTypes.Created);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
