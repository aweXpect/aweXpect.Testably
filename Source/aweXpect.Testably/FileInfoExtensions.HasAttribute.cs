using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> <see cref="FileAttributes" />.
	/// </summary>
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> HasAttribute(this IThat<IFileInfo> source,
		FileAttributes expected)
	{
		if (expected == default)
		{
			throw new ArgumentException(
				"The expected file attributes must include at least one flag.", nameof(expected));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileAttributes" />.
	/// </summary>
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> DoesNotHaveAttribute(this IThat<IFileInfo> source,
		FileAttributes unexpected)
	{
		if (unexpected == default)
		{
			throw new ArgumentException(
				"The unexpected file attributes must include at least one flag.", nameof(unexpected));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, unexpected).Invert()),
			source);
	}

	private sealed class HasAttributeConstraint(string it, ExpectationGrammars grammars, FileAttributes expected)
		: ConstraintResult.WithValue<IFileInfo>(grammars),
			IValueConstraint<IFileInfo>
	{
		private FileAttributes _actualAttributes;

		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (!actual.Exists)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_actualAttributes = actual.Attributes;
			Outcome = (_actualAttributes & expected) == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has attribute ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(it).Append(" was ").Append(_actualAttributes);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have attribute ").Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}
}
