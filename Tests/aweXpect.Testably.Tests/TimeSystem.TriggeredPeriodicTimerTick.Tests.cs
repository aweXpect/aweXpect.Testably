#if FEATURE_PERIODIC_TIMER
using System.Threading;
using Testably.Abstractions.Testing;
using Testably.Abstractions.TimeSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class TriggeredPeriodicTimerTick
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventArrivesAsynchronously_ShouldSucceedWithinTimeout()
			{
				MockTimeSystem sut = new();
				IPeriodicTimer timer = sut.PeriodicTimer.New(TimeSpan.FromMilliseconds(10));

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						await timer.WaitForNextTickAsync(CancellationToken.None);
					});
					await That(sut).TriggeredPeriodicTimerTick().Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
				timer.Dispose();
			}

			[Fact]
			public async Task WhenNoEvent_ShouldFailAfterTimeout()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredPeriodicTimerTick().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a periodic timer tick at least once within 0:00.100,
					             but it was not triggered
					             """);
			}
		}
	}
}
#endif
