using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public partial class FileResult<TFileSystem>(
	ExpectationBuilder expectationBuilder,
	IThat<TFileSystem> subject,
	string path)
	: AndOrResult<IFileSystem, IThat<TFileSystem>>(expectationBuilder, subject)
	where TFileSystem : IFileSystem
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Verifies that the file content…
	/// </summary>
	public Content WithContent()
		=> new(_expectationBuilder, this, path);

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> string content.
	/// </summary>
	public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> WithContent(
		string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
			_expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
				=> new HasStringContentEqualToConstraint(expectationBuilder, it, grammar, path, options, expected)),
			this, options);
	}

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> binary content.
	/// </summary>
	public AndOrResult<TFileSystem, FileResult<TFileSystem>> WithContent(
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> new(
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasBinaryContentEqualToConstraint(it, grammars, path, expected, doNotPopulateThisValue)),
			this);

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
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasTimeConstraint(it, grammars, path,
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
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasTimeConstraint(it, grammars, path,
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
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasTimeConstraint(it, grammars, path,
					f => f.LastWriteTime, tolerance,
					expected, "last write time")),
			this, tolerance);
	}

	private sealed class HasTimeConstraint(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<IFileInfo, DateTime> timeAccessor,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
	{
		private DateTime _actualTime;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			IFileInfo? fileInfo = actual.FileInfo.New(path);
			_actualTime = timeAccessor(fileInfo);
			if (expected.Kind == DateTimeKind.Utc && _actualTime.Kind == DateTimeKind.Local)
			{
				_actualTime = _actualTime.ToUniversalTime();
			}

			if (expected.Kind == DateTimeKind.Local && _actualTime.Kind == DateTimeKind.Utc)
			{
				_actualTime = _actualTime.ToLocalTime();
			}

			Outcome = IsWithinTolerance(tolerance.Tolerance, _actualTime - expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("with ").Append(expectedString).Append(" equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, _actualTime);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("with ").Append(expectedString).Append(" not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private static bool IsWithinTolerance(TimeSpan? tolerance, TimeSpan difference)
		{
			if (tolerance == null)
			{
				return difference == TimeSpan.Zero;
			}

			return difference <= tolerance.Value &&
			       difference >= tolerance.Value.Negate();
		}
	}
}
