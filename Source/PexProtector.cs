using System;

internal sealed class __ProtectAttribute : Attribute { }

namespace Moq
{
	[__Protect]
	internal static class PexProtector
	{
		public static void Invoke(Action action)
		{
			action();
		}

		public static T Invoke<T>(Func<T> function)
		{
			return function();
		}
	}
}
