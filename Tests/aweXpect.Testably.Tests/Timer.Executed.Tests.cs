using System.Threading;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;
// ReSharper disable UseAwaitUsing

namespace aweXpect.Testably.Tests;

public sealed class Timer
{
	public sealed class Executed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAutoAdvanceTriggersExecutions_ShouldSucceedSynchronously()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExecutionCountReachesThreshold_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(3, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInsufficientExecutions_ShouldFailAfterTimeout()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Within(TimeSpan.FromMilliseconds(100)).Exactly(3.Times());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed exactly 3 times within 0:00.100,
					             but it was not executed
					             """);
			}

			[Fact]
			public async Task WhenLiveExecutionsArriveDuringWithin_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					// ReSharper disable once AccessToDisposedClosure
					sut.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(5));
				});

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(2.Times()).Within(TimeSpan.FromSeconds(5));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ITimerMock? sut = null;

				async Task Act()
				{
					await That(sut!).Executed().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithoutQuantifier_WhenExecutedAtLeastOnce_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(1, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
