using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Throws</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IThrows : IHideObjectMembers
	{
		/// <summary>
		/// Specifies the exception to throw when the method is invoked.
		/// </summary>
		/// <param name="exception">Exception instance to throw.</param>
		/// <example>
		/// This example shows how to throw an exception when the method is 
		/// invoked with an empty string argument:
		/// <code>
		/// mock.Expect(x =&gt; x.Execute(""))
		///     .Throws(new ArgumentException());
		/// </code>
		/// </example>
		IThrowsResult Throws(Exception exception);

		/// <summary>
		/// Specifies the type of exception to throw when the method is invoked.
		/// </summary>
		/// <typeparam name="TException">Type of exception to instantiate and throw when the expectation is met.</typeparam>
		/// <example>
		/// This example shows how to throw an exception when the method is 
		/// invoked with an empty string argument:
		/// <code>
		/// mock.Expect(x =&gt; x.Execute(""))
		///     .Throws&lt;ArgumentException&gt;();
		/// </code>
		/// </example>
		IThrowsResult Throws<TException>() where TException : Exception, new();
	}
}
