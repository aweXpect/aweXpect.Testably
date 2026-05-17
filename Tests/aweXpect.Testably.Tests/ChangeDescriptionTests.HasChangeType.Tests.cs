using System.IO;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasChangeType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveChangeType_WhenChangeTypeDiffers_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHaveChangeType_WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected");
			}

			[Fact]
			public async Task WhenChangeTypeDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

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
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(WatcherChangeTypes.Created);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected");
			}
		}
	}
}
