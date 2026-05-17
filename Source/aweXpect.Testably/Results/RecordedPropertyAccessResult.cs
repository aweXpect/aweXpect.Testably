using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result of a recorded property access assertion.
/// </summary>
/// <remarks>
///     Inherits the count vocabulary (<c>Once</c>, <c>Twice</c>, <c>Never</c>, <c>Exactly</c>,
///     <c>AtLeast</c>, <c>AtMost</c>, …) from <see cref="CountResult{TType,TThat,TSelf}" />.
/// </remarks>
public sealed class RecordedPropertyAccessResult
	: CountResult<IFileSystemStatistics, IThat<IFileSystemStatistics>, RecordedPropertyAccessResult>
{
	internal RecordedPropertyAccessResult(
		ExpectationBuilder expectationBuilder,
		IThat<IFileSystemStatistics> subject,
		Quantifier quantifier)
		: base(expectationBuilder, subject, quantifier)
	{
	}
}
