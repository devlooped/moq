using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
