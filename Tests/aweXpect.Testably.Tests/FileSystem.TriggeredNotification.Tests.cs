using System.IO;
using aweXpect.Core;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class TriggeredNotification
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPriorEventExists_ShouldSucceedSynchronously()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act() => await That(sut).TriggeredNotification();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoPriorEvent_ShouldFailAfterTimeout()
			{
				MockFileSystem sut = new();

				async Task Act()
					=> await That(sut).TriggeredNotification().Within(TimeSpan.FromMilliseconds(100));

				await That(Act).ThrowsException()
					.WithMessage("*triggered a notification*")
					.AsWildcard();
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventMatches_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
					=> await That(sut).TriggeredNotification(c => c.Name == "foo.txt");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventDoesNotMatch_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
					=> await That(sut).TriggeredNotification(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));

				await That(Act).ThrowsException()
					.WithMessage("*triggered a notification*matching*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenEventArrivesAsynchronously_ShouldSucceedWithinTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
					=> await That(sut).TriggeredNotification().Within(TimeSpan.FromSeconds(2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichExposesCapturedChanges()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
					=> await That(sut).TriggeredNotification().Which.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatching_NarrowsAssertion()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
					=> await That(sut).TriggeredNotification()
						.Matching(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredTwice_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");

				async Task Act()
					=> await That(sut).TriggeredNotification()
						.Matching(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));

				await That(Act).ThrowsException()
					.WithMessage("*triggered twice*")
					.AsWildcard();
			}
		}
	}
}
