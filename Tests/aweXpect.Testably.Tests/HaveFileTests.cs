using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public class HaveFileTests
{
	[Fact]
	public async Task WhenFileIsMissing_ShouldFail()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();

		async Task Act()
			=> await That(sut).Should().HaveFile(path);

		await That(Act).Should().ThrowException()
			.WithMessage($"""
			              Expected sut to
			              have file '{path}',
			              but it did not exist
			              """);
	}

	[Fact]
	public async Task WhenPathIsDirectory_ShouldFail()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();
		sut.Directory.CreateDirectory(path);

		async Task Act()
			=> await That(sut).Should().HaveFile(path);

		await That(Act).Should().ThrowException()
			.WithMessage($"""
			              Expected sut to
			              have file '{path}',
			              but it was a directory
			              """);
	}

	[Fact]
	public async Task WhenFileExists_ShouldSucceed()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();
		// ReSharper disable once MethodHasAsyncOverload
		sut.File.WriteAllText(path, "");

		async Task Act()
			=> await That(sut).Should().HaveFile(path);

		await That(Act).Should().NotThrow();
	}
}
