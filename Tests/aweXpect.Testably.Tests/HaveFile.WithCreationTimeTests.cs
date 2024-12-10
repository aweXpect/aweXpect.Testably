using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public partial class HaveFile
{
	public class WithCreationTimeTests
	{
		[Fact]
		public async Task WhenCreationTimeDiffers_WithLocalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}

		[Fact]
		public async Task WhenCreationTimeDiffers_WithUniversalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}

		[Fact]
		public async Task WhenCreationTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenCreationTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}


		[Fact]
		public async Task WhenCreationTimeMatches_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTime(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime);

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenCreationTimeMatches_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetCreationTimeUtc(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithCreationTime(expectedTime);

			await That(Act).Should().NotThrow();
		}
	}
}
