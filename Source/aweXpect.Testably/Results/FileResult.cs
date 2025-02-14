using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public class FileResult<TFileSystem>(
	ExpectationBuilder expectationBuilder,
	IThat<TFileSystem> subject,
	string path)
	: AndOrResult<IFileSystem, IThat<TFileSystem>>(expectationBuilder, subject)
	where TFileSystem : IFileSystem
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> string content.
	/// </summary>
	public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> WithContent(
		string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammar)
				=> new HasContentConstraint(it, path, options, expected)),
			this, options);
	}

	/// <summary>
	///     Verifies that the string content of the file satisfies the <paramref name="expectations" />.
	/// </summary>
	public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> WhoseContent(
		Action<IThat<string?>> expectations)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder
				.ForMember(
					MemberAccessor<TFileSystem, string>.FromFunc(f => f.File.ReadAllText(path), "content "),
					(member, expectation) => expectation.Append(" whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<string?>(e))),
			this, options);
	}

	/// <summary>
	///     Verifies that the creation time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TFileSystem, FileResult<TFileSystem>> WithCreationTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammar)
				=> new HasTimeConstraint(it, path,
					f => f.CreationTime, tolerance,
					expected, "creation time")),
			this, tolerance);
	}

	/// <summary>
	///     Verifies that the last access time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastAccessTime" /> or <see cref="IFileSystemInfo.LastAccessTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TFileSystem, FileResult<TFileSystem>> WithLastAccessTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammar)
				=> new HasTimeConstraint(it, path,
					f => f.LastAccessTime, tolerance,
					expected, "last access time")),
			this, tolerance);
	}

	/// <summary>
	///     Verifies that the last write time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastWriteTime" /> or <see cref="IFileSystemInfo.LastWriteTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TFileSystem, FileResult<TFileSystem>> WithLastWriteTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammar)
				=> new HasTimeConstraint(it, path,
					f => f.LastWriteTime, tolerance,
					expected, "last write time")),
			this, tolerance);
	}

	private readonly struct HasTimeConstraint(
		string it,
		string path,
		Func<IFileInfo, DateTime> timeAccessor,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			IFileInfo? fileInfo = actual.FileInfo.New(path);
			DateTime time = timeAccessor(fileInfo);
			if (expected.Kind == DateTimeKind.Utc && time.Kind == DateTimeKind.Local)
			{
				time = time.ToUniversalTime();
			}

			if (expected.Kind == DateTimeKind.Local && time.Kind == DateTimeKind.Utc)
			{
				time = time.ToLocalTime();
			}

			if (IsWithinTolerance(tolerance.Tolerance, time - expected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} was {Formatter.Format(time)}");
		}

		private static bool IsWithinTolerance(TimeSpan? tolerance, TimeSpan difference)
		{
			if (tolerance == null)
			{
				return difference == TimeSpan.Zero;
			}

			return difference <= tolerance.Value &&
			       difference >= tolerance.Value.Negate();
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with {expectedString} equal to {Formatter.Format(expected)}{tolerance}";
	}

	private readonly struct HasContentConstraint(
		string it,
		string path,
		StringEqualityOptions options,
		string expected)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			string content = actual.File.ReadAllText(path);
			if (options.AreConsideredEqual(content, expected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} was {Formatter.Format(content)} which {new StringDifference(content, expected)}");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content {Formatter.Format(expected)}";
	}
}
