using System.IO;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class DidNotTriggerNotification
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActionDoesNotTrigger_ShouldSucceed()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification(ct =>
					{
						bool _ = sut.Directory.Exists("/");
						return Task.CompletedTask;
					}).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActionTriggers_ShouldFail()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification(_ =>
					{
						sut.File.WriteAllText("foo.txt", "x");
						return Task.CompletedTask;
					}).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a notification*")
					.AsWildcard();
			}

			[Fact]
			public async Task WithMatchingPredicate_WhenOnlyNonMatchingTriggered_ShouldSucceed()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification(_ =>
						{
							sut.File.WriteAllText("foo.txt", "x");
							return Task.CompletedTask;
						}).Matching(c => c.ChangeType == WatcherChangeTypes.Deleted)
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
