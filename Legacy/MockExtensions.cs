using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Moq.Language.Flow;
using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Extensions for backwards compatibility with legacy test code 
	/// that uses older features of the API.
	/// </summary>
	public static class IMockExtensions
	{
		#region Legacy Expect


		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a void method.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same method or property, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <param name="mock">The mock where the expectation will be set.</param>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example group="expectations">
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Expect(x =&gt; x.Execute("ping"));
		/// </code>
		/// </example>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetup Expect<T>(this Mock<T> mock, Expression<Action<T>> expression)
			where T : class
		{
			return Mock.SetUpExpect<T>(expression, mock.Interceptor);
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a value returning method.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mocked interface or class.</typeparam>
		/// <remarks>
		/// If more than one expectation is set for the same method or property, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <param name="mock">The mock where the expectation will be set.</param>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example group="expectations">
		/// <code>
		/// mock.Expect(x =&gt; x.HasInventory("Talisker", 50)).Returns(true);
		/// </code>
		/// </example>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetup<TResult> Expect<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression)
			where T : class
		{
			return Mock.SetUpExpect(expression, mock.Interceptor);
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a property getter.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same property getter, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mocked interface or class.</typeparam>
		/// <param name="mock">The mock where the expectation will be set.</param>
		/// <param name="expression">Lambda expression that specifies the expected property getter.</param>
		/// <example group="expectations">
		/// <code>
		/// mock.ExpectGet(x =&gt; x.Suspended)
		///     .Returns(true);
		/// </code>
		/// </example>
		[Obsolete("ExpectGet has been renamed to SetupGet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupGetter<TProperty> ExpectGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			return Mock.SetUpExpectGet(expression, mock.Interceptor);
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a property setter.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same property setter, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mocked interface or class.</typeparam>
		/// <param name="mock">The mock where the expectation will be set.</param>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
		/// <example group="expectations">
		/// <code>
		/// mock.ExpectSet(x =&gt; x.Suspended);
		/// </code>
		/// </example>
		[Obsolete("ExpectSet has been renamed to SetupSet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<TProperty> ExpectSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			return Mock.SetUpExpectSet<T, TProperty>(expression, mock.Interceptor);
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a property setter with a specific value.
		/// </summary>
		/// <remarks>
		/// More than one expectation can be set for the setter with 
		/// different values.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mocked interface or class.</typeparam>
		/// <param name="mock">The mock where the expectation will be set.</param>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
		/// <param name="value">The value expected to be set for the property.</param>
		/// <example group="expectations">
		/// <code>
		/// mock.ExpectSet(x =&gt; x.Suspended, true);
		/// </code>
		/// </example>
		[Obsolete("ExpectSet has been renamed to SetupSet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISetupSetter<TProperty> ExpectSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			return Mock.SetUpExpectSet(expression, value, mock.Interceptor);
		}

		#endregion

		/// <summary>
		/// Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation and marks it as verifiable. After 
		/// the mock is used, a <c>Verify()</c> call is issued on the mock 
		/// to ensure the method in the setup was invoked:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Setup(x =&gt; x.HasInventory(TALISKER, 50)).Verifiable().Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory.
		/// mock.Verify();
		/// </code>
		/// </example>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		[Obsolete("To verify invocations, use Verify passing an explicit expression instead (i.e. mock.Verify(m => m.Do());).", false)]
		public static void Verify(this Mock mock)
		{
			try
			{
				mock.Interceptor.Verify();
				foreach (var inner in mock.InnerMocks.Values)
				{
					inner.Verify();
				}
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				// TODO: see how to mangle the stacktrace so 
				// that the mock doesn't even show up there.
				throw ex;
			}
		}

		/// <summary>
		/// Verifies all expectations regardless of whether they have 
		/// been flagged as verifiable.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation without marking it as verifiable. After 
		/// the mock is used, a <see cref="VerifyAll"/> call is issued on the mock 
		/// to ensure that all expectations are met:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Expect(x =&gt; x.HasInventory(TALISKER, 50)).Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory, even 
		/// // that expectation was not marked as verifiable.
		/// mock.VerifyAll();
		/// </code>
		/// </example>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		[Obsolete("To verify invocations, use Verify passing an explicit expression instead (i.e. mock.Verify(m => m.Do());).", false)]
		public static void VerifyAll(this Mock mock)
		{
			// Made static so it can be called from As<TInterface>
			try
			{
				mock.Interceptor.VerifyAll();
				foreach (var inner in mock.InnerMocks.Values)
				{
					inner.VerifyAll();
				}
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				throw ex;
			}
		}
	}
}
