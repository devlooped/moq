using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Moq.Language.Flow;
using System.Linq.Expressions;

namespace Moq
{
	// Keeps legacy members that hidden and are provided 
	// for backwards compatibility (so that existing projects 
	// still compile, but people don't see them).
	// When a reference to Moq.Legacy.dll is added to a projects, 
	public partial class Mock
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
		/// this.Setup(x =&gt; x.HasInventory(TALISKER, 50)).Verifiable().Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory.
		/// this.Verify();
		/// </code>
		/// </example>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to explicitly reset the stack trace here.")]
		[Obsolete("To verify invocations, use Verify passing an explicit expression instead (i.e. this.Verify(m => m.Do());).", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Verify()
		{
			try
			{
				this.Interceptor.Verify();
				foreach (var inner in this.InnerMocks.Values)
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
		/// this.Setup(x =&gt; x.HasInventory(TALISKER, 50)).Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory, even 
		/// // that expectation was not marked as verifiable.
		/// this.VerifyAll();
		/// </code>
		/// </example>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to explicitly reset the stack trace here.")]
		[Obsolete("To verify invocations, use Verify passing an explicit expression instead (i.e. this.Verify(m => m.Do());).", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void VerifyAll()
		{
			try
			{
				this.Interceptor.VerifyAll();
				foreach (var inner in this.InnerMocks.Values)
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
