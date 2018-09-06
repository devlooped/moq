// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

internal sealed class __ProtectAttribute : Attribute
{
}

namespace Moq
{
	[__Protect]
	[DebuggerStepThrough]
	internal static class PexProtector
	{
		[DebuggerHidden]
		public static void Invoke(Action action)
		{
			action();
		}

		[DebuggerHidden]
		public static void Invoke<T1>(Action<T1> action, T1 arg1)
		{
			action(arg1);
		}

		[DebuggerHidden]
		public static void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			action(arg1, arg2);
		}

		[DebuggerHidden]
		public static TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3)
		{
			return function(arg1, arg2, arg3);
		}
	}
}
