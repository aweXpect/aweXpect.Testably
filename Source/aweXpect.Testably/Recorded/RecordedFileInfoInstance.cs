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
///     Assertions on recorded calls against a single <see cref="IFileInfo" /> instance (identified by path).
/// </summary>
public sealed class RecordedFileInfoInstance
{
	private readonly IThat<IFileSystemStatistics> _subject;
	private readonly string _path;
	private readonly string _bucketDescription;

	internal RecordedFileInfoInstance(IThat<IFileSystemStatistics> subject, string path)
	{
		_subject = subject;
		_path = path;
		_bucketDescription = $"FileInfo[\"{path}\"]";
	}

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.AppendText()" />.
	/// </summary>
	public RecordedMethodCallResult AppendText()
		=> Build(nameof(IFileInfo.AppendText));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Create()" />.
	/// </summary>
	public RecordedMethodCallResult Create()
		=> Build(nameof(IFileInfo.Create));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.CreateText()" />.
	/// </summary>
	public RecordedMethodCallResult CreateText()
		=> Build(nameof(IFileInfo.CreateText));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemInfo.Delete" />.
	/// </summary>
	public RecordedMethodCallResult Delete()
		=> Build(nameof(IFileSystemInfo.Delete));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.MoveTo(string)" /> and overloads.
	/// </summary>
	public RecordedMethodCallResult MoveTo(
		Func<string, bool>? destFileName = null,
		Func<bool, bool>? overwrite = null,
		[CallerArgumentExpression(nameof(destFileName))] string? destFileNameExpression = null,
		[CallerArgumentExpression(nameof(overwrite))] string? overwriteExpression = null)
		=> Build(nameof(IFileInfo.MoveTo),
			ParameterMatcher.From("destFileName", destFileName, destFileNameExpression),
			ParameterMatcher.From("overwrite", overwrite, overwriteExpression));

	/// <summary>
	///     Recorded accesses to <see cref="IFileInfo.IsReadOnly" />.
	/// </summary>
	public RecordedProperty IsReadOnly
		=> new(_subject, s => s.FileInfo[_path], _bucketDescription, nameof(IFileInfo.IsReadOnly));

	/// <summary>
	///     Recorded accesses to <see cref="System.IO.FileSystemInfo.Attributes" /> via <see cref="IFileInfo" />.
	/// </summary>
	public RecordedProperty Attributes
		=> new(_subject, s => s.FileInfo[_path], _bucketDescription, nameof(IFileSystemInfo.Attributes));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		string path = _path;
		string bucketDescription = _bucketDescription;
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, quantifier,
					s => s.FileInfo[path], bucketDescription, methodName, matchers)),
			_subject, quantifier);
	}
}
