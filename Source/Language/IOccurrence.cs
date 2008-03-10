using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Language
{
	/// <summary>
	/// Defines occurrence members to constraint expectations.
	/// </summary>
	public interface IOccurrence : IHideObjectMembers
	{
		/// <summary>
		/// The expected invocation can happen at most once.
		/// </summary>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Expect(foo => foo.Execute("ping"))
		///     .AtMostOnce();
		/// </code>
		/// </example>
		IVerifies AtMostOnce();
	}
}
