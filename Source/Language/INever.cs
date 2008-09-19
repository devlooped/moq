using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Never</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface INever
	{
		/// <summary>
		/// The expected invocation is never expected to happen.
		/// </summary>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Expect(foo => foo.Execute("ping"))
		///     .Never();
		/// </code>
		/// </example>
		/// <remarks>
		/// <see cref="Never"/> is always verified inmediately as 
		/// the invocations are performed, like strict mocks do 
		/// with unexpected invocations.
		/// </remarks>
		void Never();
	}
}
