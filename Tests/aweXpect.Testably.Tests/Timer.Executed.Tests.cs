using System.Threading;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class Timer
{
	public sealed class Executed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAutoAdvanceTriggersExecutions_ShouldSucceedSynchronously()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));

				async Task Act()
				{
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();

				sut.Dispose();
			}

			[Fact]
			public async Task WhenExecutionCountReachesThreshold_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(3, 5000);

				async Task Act()
				{
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();

				sut.Dispose();
			}

			[Fact]
			public async Task WhenInsufficientExecutions_ShouldFailAfterTimeout()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				async Task Act()
				{
					await That(sut).Executed(3.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed exactly 3 times within 0:00.100,
					             but it was not executed
					             """);

				sut.Dispose();
			}

			[Fact]
			public async Task WhenLiveExecutionsArriveDuringWithin_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(5));
				});

				async Task Act()
				{
					await That(sut).Executed().AtLeast(2.Times()).Within(TimeSpan.FromSeconds(5));
				}

				await That(Act).DoesNotThrow();

				sut.Dispose();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ITimerMock? sut = null;

				async Task Act()
				{
					await That(sut!).Executed(1.Times()).Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed exactly once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithoutQuantifier_WhenExecutedAtLeastOnce_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(1, 5000);

				async Task Act()
				{
					await That(sut).Executed().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();

				sut.Dispose();
			}
		}
	}
}
