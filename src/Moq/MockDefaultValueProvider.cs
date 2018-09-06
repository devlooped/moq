// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq
{
	/// <summary>
	/// A <see cref="DefaultValueProvider"/> that returns an empty default value 
	/// for non-mockable types, and mocks for all other types (interfaces and
	/// non-sealed classes) that can be mocked.
	/// </summary>
	internal sealed class MockDefaultValueProvider : LookupOrFallbackDefaultValueProvider
	{
		internal MockDefaultValueProvider()
		{
		}

		internal override DefaultValue Kind => DefaultValue.Mock;

		protected override object GetFallbackDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			var emptyValue = DefaultValueProvider.Empty.GetDefaultValue(type, mock);
			if (emptyValue != null)
			{
				return emptyValue;
			}
			else if (type.IsMockeable())
			{
				// Create a new mock to be placed to InnerMocks dictionary if it's missing there
				var mockType = typeof(Mock<>).MakeGenericType(type);
				Mock newMock = (Mock)Activator.CreateInstance(mockType, mock.Behavior);
				newMock.DefaultValueProvider = mock.DefaultValueProvider;
				newMock.CallBase = mock.CallBase;
				newMock.Switches = mock.Switches;
				return newMock.Object;
			}
			else
			{
				return null;
			}
		}
	}
}
