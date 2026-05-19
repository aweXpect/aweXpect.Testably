using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class TriggeredDateTimeRead
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
						_ = sut.DateTime.Now;
					});
					await That(sut).TriggeredDateTimeRead().Within(TimeSpan.FromSeconds(5));
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
					await That(sut).TriggeredDateTimeRead().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a DateTime read at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithPredicate_FiltersToMatchingReads()
			{
				DateTime fixedTime = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				MockTimeSystem sut = new(fixedTime);

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						_ = sut.DateTime.UtcNow;
					});
					await That(sut).TriggeredDateTimeRead(d => d == fixedTime)
						.Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
