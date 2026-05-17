using System.IO;
using Testably.Abstractions.Testing;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasOldPath
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenOldPathDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				TFsChangeDescription? renamed = null;
				using IAwaitableCallback<TFsChangeDescription> registration =
					fileSystem.Notify.OnEvent(c =>
					{
						if (c.ChangeType == WatcherChangeTypes.Renamed)
						{
							renamed ??= c;
						}
					});
				fileSystem.File.Move("foo.txt", "bar.txt");

				async Task Act()
				{
					await That(renamed!).HasOldPath("totally-different-path");
				}

				await That(Act).ThrowsException()
					.WithMessage("*has old path equal to \"totally-different-path\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldPathIsNull_AndExpectedIsNotNull_ShouldFail()
			{
				TFsChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasOldPath("foo.txt");
				}

				await That(Act).ThrowsException()
					.WithMessage("*has old path equal to \"foo.txt\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldPathMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				TFsChangeDescription? renamed = null;
				using IAwaitableCallback<TFsChangeDescription> registration =
					fileSystem.Notify.OnEvent(c =>
					{
						if (c.ChangeType == WatcherChangeTypes.Renamed)
						{
							renamed ??= c;
						}
					});
				fileSystem.File.Move("foo.txt", "bar.txt");

				async Task Act()
				{
					await That(renamed!).HasOldPath(renamed!.OldPath!);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
