using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public partial class HaveFile
{
	public class WithLastWriteTimeTests
	{
		[Fact]
		public async Task WhenLastWriteTimeDiffers_WithLocalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}

		[Fact]
		public async Task WhenLastWriteTimeDiffers_WithUniversalTime_ShouldFail()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime);

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
				              but it was {Formatter.Format(actualTime)}
				              """);
		}

		[Fact]
		public async Task WhenLastWriteTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTime(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastWriteTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			DateTime actualTime = expectedTime.AddSeconds(1);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTimeUtc(path, actualTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime)
					.Within(TimeSpan.FromSeconds(2));

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastWriteTimeIsUnspecified_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = new(2020, 2, 1, 12, 0, 0, DateTimeKind.Unspecified);
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTime(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime);

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastWriteTimeMatches_WithLocalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToLocalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTime(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime);

			await That(Act).Should().NotThrow();
		}

		[Fact]
		public async Task WhenLastWriteTimeMatches_WithUniversalTime_ShouldSucceed()
		{
			MockFileSystem sut = new();
			DateTime expectedTime = CurrentTime().ToUniversalTime();
			string path = "foo.txt";
			sut.File.WriteAllText(path, "");
			sut.File.SetLastWriteTimeUtc(path, expectedTime);

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithLastWriteTime(expectedTime);

			await That(Act).Should().NotThrow();
		}
	}
}
