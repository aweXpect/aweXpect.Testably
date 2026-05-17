using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class DidNotTriggerNotification
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNoPriorEvent_ShouldSucceed()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPriorEventExists_ShouldFailSynchronously()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification();
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a notification*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenMatchingChange_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification()
						.Which(c => c.HasName("foo.txt"));
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a notification*matching*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenNoMatchingChange_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithNullExpectation_ShouldThrowArgumentNullException()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification().Which(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation");
			}

			[Fact]
			public async Task WithPredicate_WhenMatchingPriorEvent_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification(c => c.Name == "foo.txt");
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a notification*")
					.AsWildcard();
			}

			[Fact]
			public async Task WithPredicate_WhenNoMatchingPriorEvent_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTriggerNotification(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
