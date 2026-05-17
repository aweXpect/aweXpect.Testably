using System.IO;
using Testably.Abstractions.Testing;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasOldName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenOldNameMatches_ShouldSucceed()
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
					await That(renamed!).HasOldName(renamed!.OldName!);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
