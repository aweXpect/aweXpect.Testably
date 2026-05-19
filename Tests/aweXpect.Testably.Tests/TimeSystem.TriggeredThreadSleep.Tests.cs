using aweXpect.Core;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class TriggeredThreadSleep
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventArrivesAsynchronously_ShouldSucceedWithinTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep().Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventDoesNotMatchPredicate_ShouldFailAfterTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(20);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep(t => t == TimeSpan.FromSeconds(1))
						.Within(TimeSpan.FromMilliseconds(200));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a Thread.Sleep matching t => t == TimeSpan.FromSeconds(1) at least once within 0:00.200,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenLiveEventMatchesPredicate_ShouldSucceedWithinTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep(t => t == TimeSpan.FromMilliseconds(5))
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventMatchesWhich_ShouldSucceedWithinTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep()
						.Which(t => t.IsEqualTo(TimeSpan.FromMilliseconds(5)))
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoEvent_ShouldFailAfterTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredThreadSleep().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a Thread.Sleep at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MockTimeSystem? sut = null;

				async Task Act()
				{
					await That(sut!).TriggeredThreadSleep().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a Thread.Sleep at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhichWithNullExpectation_ShouldThrowArgumentNullException()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredThreadSleep().Which(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation");
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredTwice_ShouldFail()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(20);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep()
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(500));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("*exactly once within 0:00.500,*twice*").AsWildcard();
			}

			[Fact]
			public async Task WithPredicate_NarrowsAssertion()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredThreadSleep(t => t > TimeSpan.Zero)
						.Exactly(1.Times())
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
