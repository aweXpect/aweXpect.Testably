using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasPath
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPathDiffers_ShouldFail()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasPath("totally-different-path");
				}

				await That(Act).ThrowsException()
					.WithMessage("*has path equal to \"totally-different-path\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenPathMatches_ShouldSucceed()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasPath(change.Path);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
