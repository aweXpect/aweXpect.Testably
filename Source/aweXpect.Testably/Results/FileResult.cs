using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public partial class FileResult<TParent>
	: AndOrResult<TParent, IThat<TParent>>
	where TParent : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly string _path;
	private readonly Func<TParent, (IFileSystem fs, string fullPath)> _resolver;

	internal FileResult(
		ExpectationBuilder expectationBuilder,
		IThat<TParent> subject,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_path = path;
		_resolver = resolver;
	}

	/// <summary>
	///     Verifies that the file content…
	/// </summary>
	public Content WithContent()
		=> new(_expectationBuilder, this, _resolver);

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> string content.
	/// </summary>
	public StringEqualityTypeResult<TParent, FileResult<TParent>> WithContent(
		string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
				=> new HasStringContentEqualToConstraint(expectationBuilder, it, grammar, _resolver, options, expected)),
			this, options);
	}

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> binary content.
	/// </summary>
	public AndOrResult<TParent, FileResult<TParent>> WithContent(
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> new(
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasBinaryContentEqualToConstraint(it, grammars, _resolver, expected, doNotPopulateThisValue)),
			this);

	/// <summary>
	///     Verifies that the string content of the file satisfies the <paramref name="expectations" />.
	/// </summary>
	public StringEqualityTypeResult<TParent, FileResult<TParent>> WhoseContent(
		Action<IThat<string?>> expectations)
	{
		StringEqualityOptions options = new();
		Func<TParent, (IFileSystem fs, string fullPath)> resolver = _resolver;
		return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
			_expectationBuilder
				.ForMember(
					MemberAccessor<TParent, string>.FromFunc(
						p =>
						{
							(IFileSystem fs, string fullPath) = resolver(p);
							return fs.File.ReadAllText(fullPath);
						},
						"content "),
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
	public TimeToleranceResult<TParent, FileResult<TParent>> WithCreationTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<TParent>(it, grammars, _resolver,
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
	public TimeToleranceResult<TParent, FileResult<TParent>> WithLastAccessTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<TParent>(it, grammars, _resolver,
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
	public TimeToleranceResult<TParent, FileResult<TParent>> WithLastWriteTime(
		DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<TParent>(it, grammars, _resolver,
					f => f.LastWriteTime, tolerance,
					expected, "last write time")),
			this, tolerance);
	}
}
