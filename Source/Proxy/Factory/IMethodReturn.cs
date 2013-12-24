using System;

namespace Moq.Proxy.Factory
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMethodReturn
	{
		/// <summary>
		/// Gets the <see cref="Exception"/> thrown by the invoked method if there is one.
		/// </summary>
		/// <value>The <see cref="Exception"/> thrown by the invoked method if there is one.</value>
		Exception Exception { get; }

		/// <summary>
		/// Gets the collection of output parameters. If the method has no output parameters, this is a zero-length list.
		/// </summary>
		/// <value>The collection of output parameters.</value>
		object[] OutArgs { get; }

		/// <summary>
		/// Gets the return value from the method call.
		/// </summary>
		/// <value>The return value from the method call.</value>
		/// <remarks>This value is <b>null</b> if the method has no return value.</remarks>
		object ReturnValue { get; }
	}
}