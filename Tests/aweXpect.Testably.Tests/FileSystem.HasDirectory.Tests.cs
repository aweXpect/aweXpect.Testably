﻿using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class Tests
		{
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

			[Fact]
			public async Task WhenDirectoryIsMissing_ShouldFail()
			{
				string path = "foo";
				IFileSystem sut = new MockFileSystem();

				async Task Act()
					=> await That(sut).HasDirectory(path);

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that sut
					              has directory '{path}',
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
					              Expected that sut
					              has directory '{path}',
					              but it was a file
					              """);
			}
		}
	}
}
