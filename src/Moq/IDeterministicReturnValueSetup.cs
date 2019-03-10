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
		bool IsReturnValueKnown(out object returnValue);
	}

	internal static class DeterministicReturnValueSetup
	{
		public static Mock GetInnerMock(this IDeterministicReturnValueSetup setup)
		{
			return setup.ReturnsInnerMock(out var innerMock) ? innerMock : throw new InvalidOperationException();
		}

		public static bool ReturnsInnerMock(this IDeterministicReturnValueSetup setup, out Mock mock)
		{
			if (setup.IsReturnValueKnown(out var returnValue) && Unwrap.ResultIfCompletedTask(returnValue) is IMocked mocked)
			{
				mock = mocked.Mock;
				return true;
			}
			else
			{
				mock = null;
				return false;
			}
		}

		public static MockException TryVerifyInnerMock(this IDeterministicReturnValueSetup setup, Func<Mock, MockException> verify)
		{
			if (setup.ReturnsInnerMock(out var innerMock))
			{
				var error = verify(innerMock);
				if (error?.IsVerificationError == true)
				{
					return MockException.FromInnerMockOf(setup, error);
				}
			}

			return null;
		}
	}
}
