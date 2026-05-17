using Testably.Abstractions.Testing;
using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	/// <summary>
	///     Runs <paramref name="trigger" /> on a fresh <see cref="MockFileSystem" /> and returns
	///     the first <see cref="TFsChangeDescription" /> that fired during it.
	/// </summary>
	internal static Task<TFsChangeDescription> CaptureAsync(Action<MockFileSystem> trigger)
	{
		MockFileSystem fileSystem = new();
		TFsChangeDescription? captured = null;
		using IAwaitableCallback<TFsChangeDescription> registration = fileSystem.Notify.OnEvent(c => captured ??= c);
		trigger(fileSystem);
		if (captured is null)
		{
			throw new InvalidOperationException("No notification was captured during the trigger action.");
		}

		return Task.FromResult(captured);
	}
}
