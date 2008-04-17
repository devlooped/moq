using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Base interface for <see cref="IReturns{TResult}"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturns : IHideObjectMembers
	{
	}

	/// <summary>
	/// Defines the <c>Returns</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturns<TProperty> : IReturns, IHideObjectMembers
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the method call:
		/// <code>
		/// mock.Expect(x => x.Execute("ping"))
		///     .Returns(true);
		/// </code>
		/// </example>
		IOnceVerifiesRaise Returns(TProperty value);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value when the method is called:
		/// <code>
		/// mock.Expect(x => x.Execute("ping"))
		///     .Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the method 
		/// is executed and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IOnceVerifiesRaise Returns(Func<TProperty> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The lookup list can change between invocations and the expectation 
		/// will return different values accordingly. Also, notice how the specific 
		/// string argument is retrieved by simply declaring it as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Returns((string command) => returnValues[command]);
		/// </code>
		/// </example>
		IOnceVerifiesRaise Returns<T>(Func<T, TProperty> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                     It.IsAny&lt;string&gt;(), 
		///                     It.IsAny&lt;string&gt;()))
		///     .Returns((string arg1, string arg2) => arg1 + arg2);
		/// </code>
		/// </example>
		IOnceVerifiesRaise Returns<T1, T2>(Func<T1, T2, TProperty> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((string arg1, string arg2, int arg3) => arg1 + arg2 + arg3);
		/// </code>
		/// </example>
		IOnceVerifiesRaise Returns<T1, T2, T3>(Func<T1, T2, T3, TProperty> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                       It.IsAny&lt;string&gt;(), 
		///                       It.IsAny&lt;string&gt;(), 
		///                       It.IsAny&lt;int&gt;(), 
		///                       It.IsAny&lt;bool&gt;()))
		///     .Returns((string arg1, string arg2, int arg3, bool arg4) => arg1 + arg2 + arg3 + arg4);
		/// </code>
		/// </example>
		IOnceVerifiesRaise Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TProperty> valueFunction);
	}
}
