// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Protected
{
	/// <summary>
	/// Enables the <c>Protected()</c> method on <see cref="Mock{T}"/>, 
	/// allowing setups to be set for protected members by using their 
	/// name as a string, rather than strong-typing them which is not possible 
	/// due to their visibility.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ProtectedExtension
	{
		/// <summary>
		/// Enable protected setups for the mock.
		/// </summary>
		/// <typeparam name="T">Mocked object type. Typically omitted as it can be inferred from the mock instance.</typeparam>
		/// <param name="mock">The mock to set the protected setups on.</param>
		public static IProtectedMock<T> Protected<T>(this Mock<T> mock)
			where T : class
		{
			Guard.NotNull(mock, nameof(mock));

			return new ProtectedMock<T>(mock);
		}
	}
}
