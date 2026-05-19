using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class TimeSystem
{
	public sealed class DidNotTrigger
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DidNotTriggerDateTimeRead_WhenEventOccurs_ShouldFail()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						_ = sut.DateTime.Now;
					});
					await That(sut).DidNotTriggerDateTimeRead().Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a DateTime read*was triggered*").AsWildcard();
			}

			[Fact]
			public async Task DidNotTriggerDateTimeRead_WhenNoEvent_ShouldSucceed()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerDateTimeRead().Within(TimeSpan.FromMilliseconds(50));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DidNotTriggerTaskDelay_WhenEventOccurs_ShouldFail()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						await sut.Task.Delay(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).DidNotTriggerTaskDelay().Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a Task.Delay*was triggered*").AsWildcard();
			}

			[Fact]
			public async Task DidNotTriggerThreadSleep_WhenNoEvent_ShouldSucceed()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					await That(sut).DidNotTriggerThreadSleep().Within(TimeSpan.FromMilliseconds(50));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DidNotTriggerThreadSleep_WithPredicate_WhenNonMatching_ShouldSucceed()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.Thread.Sleep(TimeSpan.FromMilliseconds(5));
					});
					await That(sut).DidNotTriggerThreadSleep(t => t == TimeSpan.FromSeconds(1))
						.Within(TimeSpan.FromMilliseconds(500));
					await firing;
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DidNotTriggerTimeChange_WhenAdvanced_ShouldFail()
			{
				MockTimeSystem sut = new();

				async Task Act()
				{
					Task firing = Task.Run(async () =>
					{
						await Task.Delay(50);
						sut.TimeProvider.AdvanceBy(TimeSpan.FromSeconds(1));
					});
					await That(sut).DidNotTriggerTimeChange().Within(TimeSpan.FromSeconds(5));
					await firing;
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger a time change*was triggered*").AsWildcard();
			}
		}
	}
}
