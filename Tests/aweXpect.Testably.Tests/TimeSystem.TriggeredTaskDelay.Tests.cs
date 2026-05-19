using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class TriggeredTaskDelay
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
						await sut.Task.Delay(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredTaskDelay().Within(TimeSpan.FromSeconds(5));
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
						await sut.Task.Delay(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredTaskDelay(t => t == TimeSpan.FromSeconds(1))
						.Within(TimeSpan.FromMilliseconds(200));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a Task.Delay matching t => t == TimeSpan.FromSeconds(1) at least once within 0:00.200,
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
						await sut.Task.Delay(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredTaskDelay(t => t == TimeSpan.FromMilliseconds(5))
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
					await That(sut).TriggeredTaskDelay().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a Task.Delay at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenMatching_ShouldSucceed()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						await sut.Task.Delay(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).TriggeredTaskDelay()
						.Which(t => t.IsEqualTo(TimeSpan.FromMilliseconds(5)))
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
