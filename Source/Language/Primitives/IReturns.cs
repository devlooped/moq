using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;

namespace Moq.Language.Primitives
{
	/// <summary>
	/// Base interface for <see cref="IReturns{TResult}"/>.
	/// </summary>
	public interface IReturns : IHideObjectMembers
	{
	}

	/// <summary>
	/// Defines the <c>Returns</c> verb.
	/// </summary>
	public interface IReturns<TResult> : IReturns, IHideObjectMembers
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the method call:
		/// <code>
		/// mock.Expect(x => x.Execute("ping")).Returns(true);
		/// </code>
		/// </example>
		IOnceVerifies Returns(TResult value);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example>
		/// Return a calculated value when the method is called:
		/// <code>
		/// mock.Expect(x => x.Execute("ping")).Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the method 
		/// is executed and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IOnceVerifies Returns(Func<TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		IOnceVerifies Returns<T>(Func<T, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		IOnceVerifies Returns<T1, T2>(Func<T1, T2, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		IOnceVerifies Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		IOnceVerifies Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction);
	}
}
