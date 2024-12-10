using System;
using aweXpect.Core;

namespace aweXpect.Testably;

internal class Subject<T>(ExpectationBuilder expectationBuilder) : IExpectSubject<T>, IThat<T>
{
	/// <inheritdoc />
	public IThat<T> Should(Action<ExpectationBuilder> builderOptions)
		=> this;

	/// <inheritdoc />
	public ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;
}