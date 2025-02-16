using System;
using aweXpect.Core;

namespace aweXpect.Testably;

internal static class Subject
{
	public static IThatIs<T> ThatIs<T>(this IThat<T> that)
	{
		if (that is IThatIs<T> thatIs)
		{
			return thatIs;
		}

		throw new NotSupportedException($"The type {that.GetType()} is not supported.");
	}
}
