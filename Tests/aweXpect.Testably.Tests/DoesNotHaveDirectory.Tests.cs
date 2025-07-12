using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed class DoesNotHaveDirectory
{
	public sealed class Tests
	{
		[Fact]
		public async Task WhenDirectoryExists_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Directory.CreateDirectory(path);

			async Task Act()
				=> await That(sut).DoesNotHaveDirectory(path);

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected that sut
				              does not have directory '{path}',
				              but it did
				              """);
		}

		[Fact]
		public async Task WhenDirectoryIsMissing_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();

			async Task Act()
				=> await That(sut).DoesNotHaveDirectory(path);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenPathIsFile_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			// ReSharper disable once MethodHasAsyncOverload
			sut.File.WriteAllText(path, "");

			async Task Act()
				=> await That(sut).DoesNotHaveDirectory(path);

			await That(Act).DoesNotThrow();
		}
	}
}
