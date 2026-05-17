using System.IO;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasNotifyFilters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsMissing_ShouldSucceed()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsPresent_ShouldFail()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters present = change.NotifyFilters;

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(present);
				}

				await That(Act).ThrowsException()
					.WithMessage("*does not have notify filters*").AsWildcard();
			}

			[Fact]
			public async Task WhenContainingExpectedFlag_ShouldSucceed()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters expected = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMissingExpectedFlag_ShouldFail()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).ThrowsException()
					.WithMessage("*has notify filters Security*").AsWildcard();
			}
		}
	}
}
