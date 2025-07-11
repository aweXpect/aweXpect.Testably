﻿using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class WithFiles
		{
			public sealed class Tests
			{
				[Fact]
				public async Task AllHaveContent_WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
						=> await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.HasContent("SOME-CONTENT")));

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory '{path}' whose files have content equal to "SOME-CONTENT" for all items,
						              but not all were

						              File content:
						              some-content
						              """);
				}

				[Fact]
				public async Task AllHaveContent_WhenContentMatches_ShouldSucceed()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
						=> await That(sut).HasDirectory(path)
							.WithFiles(f => f.All().ComplyWith(x => x.HasContent("SOME-CONTENT").IgnoringCase()));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task AllHaveContent_WhenNegated_WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
						=> await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasContent("SOME-CONTENT"))));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task AllHaveContent_WhenNegated_WhenContentMatches_ShouldFail()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
						=> await That(sut).HasDirectory(path)
							.WithFiles(f
								=> f.All().ComplyWith(x => x.DoesNotComplyWith(it => it.HasContent("some-content"))));

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory '{path}' whose files have content not equal to "some-content" for all items,
						              but not all were

						              File content:
						              some-content
						              """);
				}

				[Fact]
				public async Task BeEmpty_WhenDirectoryIsEmpty_ShouldSucceed()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path);

					async Task Act()
						=> await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task BeEmpty_WhenDirectoryIsNotEmpty_ShouldFail()
				{
					string path = "foo";
					IFileSystem sut = new MockFileSystem();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
						=> await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory '{path}' whose files are empty,
						              but files was [
						                foo{Path.DirectorySeparatorChar}bar.txt
						              ]
						              """);
				}
			}
		}
	}
}
