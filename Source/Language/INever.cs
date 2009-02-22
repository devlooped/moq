using System;
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
		/// mock.Setup(foo => foo.Execute("ping"))
		///     .Never();
		/// </code>
		/// </example>
		/// <remarks>
		/// <see cref="Never"/> is always verified inmediately as 
		/// the invocations are performed, like strict mocks do 
		/// with unexpected invocations.
		/// </remarks>
		[Obsolete("To verify this condition, use the overload to Verify that receives Times.Never().")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void Never();
	}
}
