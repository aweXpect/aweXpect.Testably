using aweXpect.Core;
using aweXpect.Testably.Recorded;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably;

/// <summary>
///     Extension methods for asserting against <see cref="IFileSystemStatistics" />.
/// </summary>
public static class StatisticsExtensions
{
	/// <summary>
	///     Entry-point for assertions on recorded calls and property accesses.
	/// </summary>
	/// <example>
	///     <code>
	/// await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
	/// await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "foo.txt").Once();
	/// </code>
	/// </example>
	public static RecordedFileSystemStatistics Recorded(this IThat<IFileSystemStatistics> subject)
		=> new(subject);
}
