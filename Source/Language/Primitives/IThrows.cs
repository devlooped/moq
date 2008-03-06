using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;

namespace Moq.Language.Primitives
{
	/// <summary>
	/// Defines the <c>Throws</c> verb.
	/// </summary>
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
		/// mock.Expect(x => x.Execute("")).Throws(new ArgumentException());
		/// </code>
		/// </example>
		IOnceVerifies Throws(Exception exception);
	}
}
