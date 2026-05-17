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
			public async Task WhenActionTriggersOnce_ShouldSucceed()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ =>
					{
						sut.File.WriteAllText("foo.txt", "x");
						return Task.CompletedTask;
					});
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMatchingPredicateNeverTriggers_ShouldFail()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ =>
						{
							sut.File.WriteAllText("foo.txt", "x");
							return Task.CompletedTask;
						}).Matching(c => c.ChangeType == WatcherChangeTypes.Deleted)
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("*triggered a notification*matching*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhichExposesCapturedChanges()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ =>
					{
						sut.File.WriteAllText("foo.txt", "x");
						return Task.CompletedTask;
					}).Which.IsNotEmpty();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredOnce_ShouldSucceed()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ =>
						{
							sut.File.WriteAllText("foo.txt", "x");
							return Task.CompletedTask;
						}).Matching(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredTwice_ShouldFail()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ =>
						{
							sut.File.WriteAllText("a.txt", "x");
							sut.File.WriteAllText("b.txt", "x");
							return Task.CompletedTask;
						}).Matching(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("*triggered twice*")
					.AsWildcard();
			}
		}
	}
}
