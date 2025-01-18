using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public class HasDirectoryTests
{
	[Fact]
	public async Task WhenDirectoryIsMissing_ShouldFail()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();

		async Task Act()
			=> await That(sut).HasDirectory(path);

		await That(Act).ThrowsException()
			.WithMessage($"""
			              Expected sut to
			              have directory '{path}',
			              but it did not exist
			              """);
	}

	[Fact]
	public async Task WhenPathIsFile_ShouldFail()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();
		// ReSharper disable once MethodHasAsyncOverload
		sut.File.WriteAllText(path, "");

		async Task Act()
			=> await That(sut).HasDirectory(path);

		await That(Act).ThrowsException()
			.WithMessage($"""
			              Expected sut to
			              have directory '{path}',
			              but it was a file
			              """);
	}

	[Fact]
	public async Task WhenDirectoryExists_ShouldSucceed()
	{
		string path = "foo";
		IFileSystem sut = new MockFileSystem();
		sut.Directory.CreateDirectory(path);

		async Task Act()
			=> await That(sut).HasDirectory(path);

		await That(Act).DoesNotThrow();
	}
}
