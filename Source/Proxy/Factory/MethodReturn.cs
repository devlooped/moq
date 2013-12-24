using System;

namespace Moq.Proxy.Factory
{
	internal class MethodReturn : IMethodReturn
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodReturn"/> class.
		/// </summary>
		/// <param name="returnValue">The return value.</param>
		/// <param name="outArgs">The output arguments.</param>
		public MethodReturn(object returnValue, params object[] outArgs)
		{
			this.ReturnValue = returnValue;
			this.OutArgs = outArgs;
		}

		/// <summary>
		/// Gets the <see cref="Exception"/> thrown by the invoked method if there is one.
		/// </summary>
		/// <value>The <see cref="Exception"/> thrown by the invoked method if there is one.</value>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Gets the collection of output parameters. If the method has no output parameters, this is a zero-length list.
		/// </summary>
		/// <value>The collection of output parameters.</value>
		public object[] OutArgs { get; private set; }

		/// <summary>
		/// Gets the return value from the method call.
		/// </summary>
		/// <value>The return value from the method call.</value>
		public object ReturnValue { get; private set; }
	}
}