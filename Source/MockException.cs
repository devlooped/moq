using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;

namespace Moq
{
	/// <summary>
	/// Exception thrown by mocks when expectations are not met, 
	/// the mock is not properly setup, etc.
	/// </summary>
	/// <remarks>
	/// A distinct exception type is provided so that exceptions 
	/// thrown by the mock can be differentiated in tests that 
	/// expect other exceptions to be thrown (i.e. ArgumentException).
	/// <para>
	/// Richer exception hierarchy/types are not provided as 
	/// tests typically should <b>not</b> catch or expect exceptions 
	/// from the mocks. These are typically the result of changes 
	/// in the tested class or its collaborators implementation, and 
	/// result in fixes in the mock setup so that they dissapear and 
	/// allow the test to pass.
	/// </para>
	/// </remarks>
	[Serializable]
	public class MockException : Exception
	{
		/// <summary>
		/// Made internal as it's of no use for 
		/// consumers, but it's important for 
		/// our own tests.
		/// </summary>
		internal enum ExceptionReason
		{
			NoExpectation,
			AbstractNoExpectation,
			InterfaceNoExpectation,
			ReturnValueNoExpectation,
		}

		ExceptionReason reason;

		internal MockException(ExceptionReason reason, MockBehavior behavior,
			IInvocation invocation)
			: this(reason, behavior, invocation, 
				Properties.Resources.ResourceManager.GetString(reason.ToString()))
		{
		}

		internal MockException(ExceptionReason reason, MockBehavior behavior,
			IInvocation invocation, string message)
			: base(GetMessage(reason, behavior, invocation, message))
		{
			this.reason = reason;
		}

		internal ExceptionReason Reason
		{
			get { return reason; }
		}

		private static string GetMessage(ExceptionReason reason, MockBehavior behavior, 
			IInvocation invocation, string message)
		{
			return String.Format(
				Properties.Resources.MockExceptionMessage, 
				invocation.Method.DeclaringType.Name,
				invocation.Method.Name,
				String.Join(", ",
					(from x in invocation.Arguments
					 select x == null ?
						"null" :
						x is string ?
							"\"" + (string)x + "\"" :
							x.ToString())
					.ToArray()
				),
				behavior, 
				message
			);
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		protected MockException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			this.reason = (ExceptionReason)info.GetValue("reason", typeof(ExceptionReason));
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		public override void GetObjectData(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("reason", reason);
		}
	}
}
