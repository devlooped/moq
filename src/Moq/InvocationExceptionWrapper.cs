using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	/// Internal type to mark invocation results as "exception occurred during execution". The type just
	/// wraps the Exception so a thrown exception can be distinguished from an <see cref="System.Exception"/>
	/// return by the invocation.
	/// </summary>
	internal readonly struct InvocationExceptionWrapper
	{
		public InvocationExceptionWrapper(Exception exception)
		{
			Exception = exception;
		}

		public Exception Exception { get; }
	}
}
