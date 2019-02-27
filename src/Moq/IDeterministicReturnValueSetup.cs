// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	/// <summary>
	///   Role interface for setups whose return value can be determined without invoking user code
	///   (which could have side effects beyond Moq's understanding and control).
	/// </summary>
	internal interface IDeterministicReturnValueSetup
	{
		object ReturnValue { get; }
	}

	internal static class DeterministicReturnValueSetup
	{
		public static Mock GetInnerMock(this IDeterministicReturnValueSetup setup)
		{
			return setup.ReturnsInnerMock(out var innerMock) ? innerMock : throw new InvalidOperationException();
		}

		public static bool ReturnsInnerMock(this IDeterministicReturnValueSetup setup, out Mock mock)
		{
			mock = (Unwrap.ResultIfCompletedTask(setup.ReturnValue) as IMocked)?.Mock;
			return mock != null;
		}

		public static void VerifyInnerMock(this IDeterministicReturnValueSetup setup, Action<Mock> verify)
		{
			if (setup.ReturnsInnerMock(out var innerMock))
			{
				try
				{
					verify(innerMock);
				}
				catch (MockException error) when (error.IsVerificationError)
				{
					throw MockException.FromInnerMockOf(setup, error);
				}
			}
		}
	}
}
