using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Moq.Language.Flow;

namespace Moq
{
	/// <summary>
	/// Provides static helper methods to retrive mocks and mocked objects 
	/// from other types.
	/// </summary>
	public static class AsMockExtensions
	{
		#region Returns

		/// <summary>
		/// Gets the mocked object instance (not the Mock itself) from the 
		/// Returns call.
		/// </summary>
		/// <typeparam name="T">Type of the mock, typically omitted as it's included in the fluent API signature.</typeparam>
		/// <param name="fluent">The fluent object in the API.</param>
		/// <returns>The mocked object instance, or an exception.</returns>
		public static T AsMocked<T>(this IReturnsResult<T> fluent)
			where T : class
		{
			return (T)(((MethodCall)fluent).mock.Object);
		}

		/// <summary>
		/// Gets the mock associated with the object.
		/// </summary>
		public static Mock<T> AsMock<T>(this IReturnsResult<T> fluent)
			where T : class
		{
			return (Mock<T>)((MethodCall)fluent).mock;
		}

		#endregion

		#region Setup

		/// <summary>
		/// Gets the mocked object instance (not the Mock itself) from the 
		/// Setup call.
		/// </summary>
		/// <typeparam name="T">Type of the mock, typically omitted as it's included in the fluent API signature.</typeparam>
		/// <param name="fluent">The fluent object in the API.</param>
		/// <returns>The mocked object instance, or an exception.</returns>
		public static T AsMocked<T>(this ISetup<T> fluent)
			where T : class
		{
			return (T)(((MethodCall)fluent).mock.Object);
		}

		/// <summary>
		/// Gets the mock associated with the object.
		/// </summary>
		public static Mock<T> AsMock<T>(this ISetup<T> fluent)
			where T : class
		{
			return (Mock<T>)((MethodCall)fluent).mock;
		}

		#endregion

		#region Callback

		/// <summary>
		/// Gets the mocked object instance (not the Mock itself) from the 
		/// Callback call.
		/// </summary>
		/// <typeparam name="T">Type of the mock, typically omitted as it's included in the fluent API signature.</typeparam>
		/// <param name="fluent">The fluent object in the API.</param>
		/// <returns>The mocked object instance, or an exception.</returns>
		public static T AsMocked<T>(this ICallbackResult fluent)
			where T : class
		{
			return (T)(((MethodCall)fluent).mock.Object);
		}

		/// <summary>
		/// Gets the mock associated with the object.
		/// </summary>
		public static Mock<T> AsMock<T>(this ICallbackResult fluent)
			where T : class
		{
			return (Mock<T>)((MethodCall)fluent).mock;
		}

		#endregion

		/// <summary>
		/// Calls <see cref="Mock.Get{T}"/> for the given instance.
		/// </summary>
		/// <returns>The mock associated with the object.</returns>
		/// <remarks>
		/// Provides an alternative to the static <see cref="Mock.Get"/> method.
		/// </remarks>
		public static Mock<T> AsMock<T>(this T @object)
			where T : class
		{
			var call = @object as MethodCall;
			// This may be called at any point in a fluent mock setup.
			if (call != null)
				return (Mock<T>)call.mock;
			else
				return Mock.Get(@object);
		}
	}
}
