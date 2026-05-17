using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against <see cref="IPath" />.
/// </summary>
/// <remarks>
///     Note: <see cref="IPath.Combine(string[])" /> (the <c>params</c> overload) records its argument as
///     <see cref="T:string[]" />, while the fixed-arity overloads record individual <see cref="string" /> params.
///     The mirror's <see cref="Combine" /> method targets the fixed-arity overloads.
/// </remarks>
public sealed class RecordedPathMethods
{
	private const string BucketDescription = "Path";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedPathMethods(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>Recorded calls to <see cref="IPath.ChangeExtension(string?, string?)" />.</summary>
	public RecordedMethodCallResult ChangeExtension(
		Func<string?, bool>? path = null,
		Func<string?, bool>? extension = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null,
		[CallerArgumentExpression(nameof(extension))] string? extensionExpression = null)
		=> Build(nameof(IPath.ChangeExtension),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("extension", extension, extensionExpression));

	/// <summary>Recorded calls to <see cref="IPath.Combine(string, string)" /> and fixed-arity overloads.</summary>
	public RecordedMethodCallResult Combine(
		Func<string, bool>? path1 = null,
		Func<string, bool>? path2 = null,
		Func<string, bool>? path3 = null,
		Func<string, bool>? path4 = null,
		[CallerArgumentExpression(nameof(path1))] string? path1Expression = null,
		[CallerArgumentExpression(nameof(path2))] string? path2Expression = null,
		[CallerArgumentExpression(nameof(path3))] string? path3Expression = null,
		[CallerArgumentExpression(nameof(path4))] string? path4Expression = null)
		=> Build(nameof(IPath.Combine),
			ParameterMatcher.From("path1", path1, path1Expression),
			ParameterMatcher.From("path2", path2, path2Expression),
			ParameterMatcher.From("path3", path3, path3Expression),
			ParameterMatcher.From("path4", path4, path4Expression));

	/// <summary>Recorded calls to <see cref="IPath.GetDirectoryName(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult GetDirectoryName(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.GetDirectoryName),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetExtension(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult GetExtension(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.GetExtension),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetFileName(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult GetFileName(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.GetFileName),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetFileNameWithoutExtension(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult GetFileNameWithoutExtension(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.GetFileNameWithoutExtension),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetFullPath(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetFullPath(
		Func<string, bool>? path = null,
		Func<string, bool>? basePath = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null,
		[CallerArgumentExpression(nameof(basePath))] string? basePathExpression = null)
		=> Build(nameof(IPath.GetFullPath),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("basePath", basePath, basePathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetInvalidFileNameChars" />.</summary>
	public RecordedMethodCallResult GetInvalidFileNameChars()
		=> Build(nameof(IPath.GetInvalidFileNameChars));

	/// <summary>Recorded calls to <see cref="IPath.GetInvalidPathChars" />.</summary>
	public RecordedMethodCallResult GetInvalidPathChars()
		=> Build(nameof(IPath.GetInvalidPathChars));

	/// <summary>Recorded calls to <see cref="IPath.GetPathRoot(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult GetPathRoot(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.GetPathRoot),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.GetRandomFileName" />.</summary>
	public RecordedMethodCallResult GetRandomFileName()
		=> Build(nameof(IPath.GetRandomFileName));

	/// <summary>Recorded calls to <see cref="IPath.GetTempFileName" />.</summary>
	public RecordedMethodCallResult GetTempFileName()
		=> Build(nameof(IPath.GetTempFileName));

	/// <summary>Recorded calls to <see cref="IPath.GetTempPath" />.</summary>
	public RecordedMethodCallResult GetTempPath()
		=> Build(nameof(IPath.GetTempPath));

	/// <summary>Recorded calls to <see cref="IPath.HasExtension(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult HasExtension(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.HasExtension),
			ParameterMatcher.From("path", path, pathExpression));

	/// <summary>Recorded calls to <see cref="IPath.IsPathRooted(string?)" /> and overloads.</summary>
	public RecordedMethodCallResult IsPathRooted(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null)
		=> Build(nameof(IPath.IsPathRooted),
			ParameterMatcher.From("path", path, pathExpression));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, quantifier,
					s => s.Path, BucketDescription, methodName, matchers)),
			_subject, quantifier);
	}
}
