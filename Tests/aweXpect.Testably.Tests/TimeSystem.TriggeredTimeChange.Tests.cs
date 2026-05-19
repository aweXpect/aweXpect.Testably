using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class TriggeredTimeChange
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
						sut.TimeProvider.AdvanceBy(TimeSpan.FromSeconds(1));
					});
					await That(sut).TriggeredTimeChange().Within(TimeSpan.FromSeconds(5));
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
					await That(sut).TriggeredTimeChange().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a time change at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithPredicate_NarrowsAssertion()
			{
				MockTimeSystem sut = new();
				DateTime baseTime = sut.DateTime.UtcNow;

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.TimeProvider.AdvanceBy(TimeSpan.FromSeconds(5));
					});
					await That(sut).TriggeredTimeChange(d => d >= baseTime.AddSeconds(5))
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
