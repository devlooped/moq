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
	public static class MockExtensions
	{
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
		[EditorBrowsable(EditorBrowsableState.Always)]
		public static void Verify(this Mock mock)
		{
			mock.Verify();
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
		[EditorBrowsable(EditorBrowsableState.Always)]
		public static void VerifyAll(this Mock mock)
		{
			mock.VerifyAll();
		}
	}
}
