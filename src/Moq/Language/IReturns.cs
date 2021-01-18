// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Returns</c> verb.
	/// </summary>
	/// <typeparam name="TMock">Mocked type.</typeparam>
	/// <typeparam name="TResult">Type of the return value from the expression.</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface IReturns<TMock, TResult> : IFluentInterface
		where TMock : class
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the method call:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns(TResult value);

		/// <summary>
		///   Specifies a function that will calculate the value to return from the method.
		///   <para>
		///     This overload is intended to be used in scenarios involving generic type argument matchers,
		///     such as <see cref="It.IsAnyType"/>. The function will receive the current <see cref="IInvocation"/>,
		///     which allows discovery of both arguments and type arguments.
		///   </para>
		///   <para>
		///     For all other use cases, you should prefer the other <c>Returns</c> overloads as they provide
		///     better static type safety.
		///   </para>
		/// </summary>
		/// <example>
		///   Mock a method to act like a generic factory method:
		///   <code>
		///     factory.Setup(m => m.Create&lt;It.IsAnyType&gt;())
		///            .Returns(new InvocationFunc(invocation =>
		///                     {
		///                         var typeArgument = invocation.Method.GetGenericArguments()[0];
		///                         return Activator.CreateInstance(typeArgument);
		///                     });
		///     var something = factory.Object.Create&lt;Something&gt;();
		///   </code>
		/// </example>
		IReturnsResult<TMock> Returns(InvocationFunc valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// This overload specifically allows you to specify a function with by-ref parameters.
		/// Those by-ref parameters can be assigned to (though you should probably do that from
		/// a <c>Callback</c> instead).
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value when the method is called:
		/// <code>
		/// delegate bool ExecuteFunc(ref Command command);
		///
		/// Command c = ...;
		/// mock.Setup(x => x.Execute(ref c))
		///     .Returns(new ExecuteFunc((ref Command command) => command.IsExecutable));
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns(Delegate valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value when the method is called:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the method 
		/// is executed and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IReturnsResult<TMock> Returns(Func<TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T">The type of the argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The lookup list can change between invocations and the setup 
		/// will return different values accordingly. Also, notice how the specific 
		/// string argument is retrieved by simply declaring it as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Returns((string command) => returnValues[command]);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T>(Func<T, TResult> valueFunction);

		/// <summary>
		/// Calls the real method of the object and returns its return value.
		/// </summary>
		/// <returns>The value calculated by the real method of the object.</returns>
		IReturnsResult<TMock> CallBase();
	}
}
