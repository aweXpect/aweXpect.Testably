using System;

namespace aweXpect.Testably.Helpers;

internal sealed class NotificationTimeoutOptions
{
	public TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(30);

	public bool IsExplicit { get; private set; }

	public void Within(TimeSpan timeout)
	{
		if (timeout < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(timeout), "The timeout must not be negative.");
		}

		Timeout = timeout;
		IsExplicit = true;
	}

	public override string ToString()
		=> IsExplicit ? $" within {Formatter.Format(Timeout)}" : "";
}
