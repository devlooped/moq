// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Linq.Expressions;

using Moq.Language.Flow;

namespace Moq
{
	static partial class MockExtensions
	{
		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetup<T> Expect<T>(this Mock<T> mock, Expression<Action<T>> expression)
			where T : class
		{
			return mock.Setup(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetup<T, TResult> Expect<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression)
			where T : class
		{
			return mock.Setup(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectGet has been renamed to SetupGet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupGetter<T, TProperty> ExpectGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			return mock.SetupGet(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectSet has been renamed to SetupSet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<T, TProperty> ExpectSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			return mock.SetupSet(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectSet has been renamed to SetupSet, and the new syntax allows you to pass the value in the expression itself, like f => f.Value = 25.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<T, TProperty> ExpectSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Resets all invocations recorded for this mock.
		/// </summary>
		/// <param name="mock">The mock whose recorded invocations should be reset.</param>
		[Obsolete("Use `mock.Invocations.Clear()` instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ResetCalls(this Mock mock) => mock.Invocations.Clear();

		/// <summary>
		/// Specifies a setup on the mocked type for a call to
		/// to a property setter, regardless of its value.
		/// </summary>
		/// <remarks>
		/// If more than one setup is set for the same property setter,
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="mock">The target mock for the setup.</param>
		/// <param name="expression">Lambda expression that specifies the property setter.</param>
		/// <example group="setups">
		/// <code>
		/// mock.SetupSet(x =&gt; x.Suspended);
		/// </code>
		/// </example>
		/// <devdoc>
		/// This method is not legacy, but must be on an extension method to avoid
		/// confusing the compiler with the new Action syntax.
		/// </devdoc>
		[Obsolete("Replaced by SetupSet(Action)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<T, TProperty> SetupSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			Guard.NotNull(expression, nameof(expression));

			var setup = Mock.SetupSet(mock, expression.AssignItIsAny(), condition: null);
			return new SetterSetupPhrase<T, TProperty>(setup);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("The new syntax allows you to pass the value in the expression itself, like f => f.Value = 25.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<T, TProperty> SetupSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regardless of its value.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used,
		/// and later we want to verify that a given invocation
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by VerifySet(Action)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			Guard.NotNull(expression, nameof(expression));

			Mock.VerifySet(mock, expression.AssignItIsAny(), Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25);", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25, failMessage);", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value, string failMessage)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, specifying a failure
		/// error message.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used,
		/// and later we want to verify that a given invocation
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, string)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, string failMessage)
			where T : class
		{
			Mock.VerifySet(mock, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regardless
		/// of the value but only the specified number of times.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used,
		/// and later we want to verify that a given invocation
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, Times)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, Times times)
			where T : class
		{
			Mock.VerifySet(mock, expression, times, null);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regardless
		/// of the value but only the specified number of times, and specifying a failure
		/// error message.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used,
		/// and later we want to verify that a given invocation
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, Times, string)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, Times times, string failMessage)
			where T : class
		{
			Mock.VerifySet(mock, expression, times, failMessage);
		}
	}
}
