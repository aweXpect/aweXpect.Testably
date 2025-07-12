using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed class DoesNotHaveFile
{
	public sealed class Tests
	{
		[Fact]
		public async Task WhenFileExists_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			// ReSharper disable once MethodHasAsyncOverload
			sut.File.WriteAllText(path, "");

			async Task Act()
				=> await That(sut).DoesNotHaveFile(path);

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected that sut
				              does not have file '{path}',
				              but it did
				              """);
		}

		[Fact]
		public async Task WhenFileIsMissing_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();

			async Task Act()
				=> await That(sut).DoesNotHaveFile(path);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenPathIsDirectory_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Directory.CreateDirectory(path);

			async Task Act()
				=> await That(sut).DoesNotHaveFile(path);

			await That(Act).DoesNotThrow();
		}
	}
}
