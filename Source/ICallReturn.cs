using System;

namespace Moq
{
	/// <summary>
	/// Exposes members available to mocked methods that do 
	/// not return a value.
	/// </summary>
	/// <remarks>
	/// <see cref="ICall"/> and <see cref="ICallReturn{TResult}"/> are the 
	/// return type of the two <c>Mock.Expect</c> overloads. These are the two interfaces that enable the rich 
	/// intellisense in Visual Studio allowing calls with the format <c>mock.Expect(...).Callback(...).Returns(...)</c>.
	/// <para>When a call to <c>Expect</c> is performed, the compiler 
	/// (and Visual Studio intellisense) will automatically pick one interface or the 
	/// other as the return types depending on the expression passed in the call: if the 
	/// expression represents a call to a void method, <see cref="ICall"/> members 
	/// will be available after the <c>Expect</c> invocation, allowing the 
	/// specification of a <see cref="ICall.Throws"/> or <see cref="ICall.Callback"/> action, but not a <c>Returns</c> one.
	/// </para>
	/// <para>
	/// On the other hand, if the expression passed to the <c>Expect</c> method represents 
	/// a call to a method that returns a value, the additional <see cref="ICallReturn{TResult}.Returns(TResult)"/> and 
	/// <see cref="ICallReturn{TResult}.Returns(Func{TResult})"/>
	/// members are available (<c>Throws</c> and <c>Callback</c> are inherited from <see cref="ICall"/>).
	/// </para>
	/// </remarks>
	public interface ICall
	{
		/// <summary>
		/// Specifies the exception to throw when the method is invoked.
		/// </summary>
		/// <param name="exception">Exception instance to throw.</param>
		/// <example>
		/// This example shows how to throw an exception when the method is 
		/// invoked with an empty string argument:
		/// <code>
		/// mock.Expect(x => x.Execute("")).Throws(new ArgumentException());
		/// </code>
		/// </example>
		void Throws(Exception exception);
		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean 
		/// value that can be used later:
		/// <code>
		/// bool called = false;
		/// mock.Expect(x => x.Execute()).Callback(() => called = true);
		/// </code>
		/// </example>
		ICall Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Argument type of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		ICall Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		ICall Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		ICall Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		ICall Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
		/// <summary>
		/// Marks the expectation as verifiable, meaning that a call 
		/// to <see cref="Mock{T}.Verify"/> will check if this particular 
		/// expectation was met.
		/// </summary>
		ICall Verifiable();
	}

	/// <summary>
	/// Exposes members available to mocked methods that 
	/// return a value.
	/// </summary>
	/// <remarks>
	/// <see cref="ICall"/> and <see cref="ICallReturn{TResult}"/> are the 
	/// return type of the two <c>Mock.Expect</c> overloads. These are the two interfaces that enable the rich 
	/// intellisense in Visual Studio allowing calls with the format <c>mock.Expect(...).Callback(...).Returns(...)</c>.
	/// <para>When a call to <c>Expect</c> is performed, the compiler 
	/// (and Visual Studio intellisense) will automatically pick one interface or the 
	/// other as the return types depending on the expression passed in the call: if the 
	/// expression represents a call to a void method, <see cref="ICall"/> members 
	/// will be available after the <c>Expect</c> invocation, allowing the 
	/// specification of a <see cref="ICall.Throws"/> or <see cref="ICall.Callback"/> action, but not a <c>Returns</c> one.
	/// </para>
	/// <para>
	/// On the other hand, if the expression passed to the <c>Expect</c> method represents 
	/// a call to a method that returns a value, the additional <see cref="ICallReturn{TResult}.Returns(TResult)"/> and 
	/// <see cref="ICallReturn{TResult}.Returns(Func{TResult})"/>
	/// members are available (<c>Throws</c> and <c>Callback</c> are inherited from <see cref="ICall"/>).
	/// </para>
	/// </remarks>
	public interface ICallReturn<TResult> : ICall
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
        ICallReturn<TResult> Returns(TResult value);
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
        ICallReturn<TResult> Returns(Func<TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		ICallReturn<TResult> Returns<T>(Func<T, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		ICallReturn<TResult> Returns<T1, T2>(Func<T1, T2, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		ICallReturn<TResult> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction);
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		ICallReturn<TResult> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction);

		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean 
		/// value that can be used later:
		/// <code>
		/// bool called = false;
		/// mock.Expect(x => x.Execute()).Callback(() => called = true).Returns(true);
		/// </code>
		/// Note that in the case of value-returning methods, after the <c>Callback</c> 
		/// call you still can specify the return value.
		/// </example>
		new ICallReturn<TResult> Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		new ICallReturn<TResult> Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		new ICallReturn<TResult> Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		new ICallReturn<TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		new ICallReturn<TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
		/// <summary>
		/// Marks the expectation as verifiable, meaning that a call 
		/// to <see cref="Mock{T}.Verify"/> will check if this particular 
		/// expectation was met.
		/// </summary>
		new ICallReturn<TResult> Verifiable();
	}
}
