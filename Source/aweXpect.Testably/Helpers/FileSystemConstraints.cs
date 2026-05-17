using System;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Testably.Helpers;

internal static class FileSystemConstraints
{
	internal sealed class ExistsConstraint<TInfo>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TInfo>(grammars),
			IValueConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		public ConstraintResult IsMetBy(TInfo actual)
		{
			Actual = actual;
			Outcome = actual.Exists ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("exists");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not exist");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}

	internal sealed class HasNameConstraint<TInfo>(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<TInfo>(grammars),
			IAsyncConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		private string? _actualName;

		public async Task<ConstraintResult> IsMetBy(TInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_actualName = actual.Name;
			Outcome = await options.AreConsideredEqual(_actualName, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(it, Grammars, _actualName, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}

	internal sealed class HasFileConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: ConstraintResult.WithValue<TParent>(grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private IFileSystem? _fs;
		private string? _fullPath;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			(_fs, _fullPath) = resolver(actual);
			Outcome = _fs.File.Exists(_fullPath) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has file '").Append(path).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fs?.Directory.Exists(_fullPath) == true)
			{
				stringBuilder.Append(it).Append(" was a directory");
			}
			else
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have file '").Append(path).Append('\'');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}

	internal sealed class HasDirectoryConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: ConstraintResult.WithValue<TParent>(grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private IFileSystem? _fs;
		private string? _fullPath;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			(_fs, _fullPath) = resolver(actual);
			Outcome = _fs.Directory.Exists(_fullPath) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has directory '").Append(path).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fs?.File.Exists(_fullPath) == true)
			{
				stringBuilder.Append(it).Append(" was a file");
			}
			else
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have directory '").Append(path).Append('\'');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}

	internal sealed class HasTimeConstraint<TActual>(
		string it,
		ExpectationGrammars grammars,
		Func<TActual, DateTime> timeAccessor,
		Func<TActual, bool>? existsCheck,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString,
		string normalVerb,
		string negatedVerb,
		string negatedConnector)
		: ConstraintResult.WithValue<TActual>(grammars),
			IValueConstraint<TActual>
		where TActual : class
	{
		private DateTime _actualTime;
		private bool _existed = true;

		public ConstraintResult IsMetBy(TActual actual)
		{
			Actual = actual;
			if (existsCheck != null && !existsCheck(actual))
			{
				_existed = false;
				Outcome = Outcome.Failure;
				return this;
			}

			_actualTime = timeAccessor(actual);
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
			stringBuilder.Append(normalVerb).Append(' ').Append(expectedString).Append(" equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_existed)
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(it).Append(" was ");
				Formatter.Format(stringBuilder, _actualTime);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(negatedVerb).Append(' ').Append(expectedString).Append(negatedConnector);
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
