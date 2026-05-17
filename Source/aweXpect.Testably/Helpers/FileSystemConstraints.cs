using System;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Testably.Helpers;

internal static class FileSystemConstraints
{
	internal sealed class HasTimeConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver,
		Func<IFileSystemInfo, DateTime> timeAccessor,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString)
		: ConstraintResult.WithValue<TParent>(grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private DateTime _actualTime;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			(IFileSystem fs, string fullPath) = resolver(actual);
			IFileSystemInfo info = fs.FileInfo.New(fullPath);
			_actualTime = timeAccessor(info);
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

	internal sealed class HasInfoTimeConstraint<TInfo>(
		string it,
		ExpectationGrammars grammars,
		Func<TInfo, DateTime> timeAccessor,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString)
		: ConstraintResult.WithValue<TInfo>(grammars),
			IValueConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		private DateTime _actualTime;

		public ConstraintResult IsMetBy(TInfo actual)
		{
			Actual = actual;
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
			stringBuilder.Append("has ").Append(expectedString).Append(" equal to ");
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
			stringBuilder.Append("does not have ").Append(expectedString).Append(" equal to ");
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
