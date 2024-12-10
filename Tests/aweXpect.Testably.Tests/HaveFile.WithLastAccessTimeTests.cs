using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public partial class HaveFile
{
	public class WithLastAccessTimeTests
	{
		[Fact]
		public async Task WhenLastAccessTimeDiffers_WithLocalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}

		[Fact]
		public async Task WhenLastAccessTimeDiffers_WithUniversalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}
		[Fact]
		public async Task WhenLastAccessTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastAccessTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}


		[Fact]
		public async Task WhenLastAccessTimeMatches_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTime(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime);

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastAccessTimeMatches_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastAccessTimeUtc(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastAccessTime(expectedTime);

			await That(Act).Should().NotThrow();
		}
	}
}
