﻿using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasDirectory
{
	public class WithDirectoriesTests
	{
		[Fact]
		public async Task WhenItemCountDiffers_ShouldFail()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithSubdirectory("directory1")
				.WithSubdirectory("directory2"));

			async Task Act()
				=> await That(sut).HasDirectory(path).WithDirectories(f => f.Has().Exactly(3).Items());

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected sut to
				              have directory '{path}' which subdirectories should have exactly 3 items,
				              but found only 2
				              """);
		}

		[Fact]
		public async Task WhenItemCountMatches_ShouldSucceed()
		{
			string path = "foo";
			IFileSystem sut = new MockFileSystem();
			sut.Initialize().WithSubdirectory(path).Initialized(d => d
				.WithSubdirectory("directory1")
				.WithSubdirectory("directory2"));

			async Task Act()
				=> await That(sut).HasDirectory(path).WithDirectories(f => f.Has().Exactly(2).Items());

			await That(Act).DoesNotThrow();
		}
	}
}