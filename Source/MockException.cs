//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the Moq Team nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Castle.Core.Interceptor;

namespace Moq
{
	/// <summary>
	/// Exception thrown by mocks when setups are not matched, 
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
			NoSetup,
			ReturnValueRequired,
			VerificationFailed, 
			MoreThanOneCall,
			MoreThanNCalls,
			SetupNever,
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

		internal MockException(ExceptionReason reason, string exceptionMessage)
			: base(exceptionMessage)
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
				invocation.Format(),
				behavior,
				message
			);
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
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
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		public override void GetObjectData(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("reason", reason);
		}
	}

	/// <devdoc>
	/// Used by the mock factory to accumulate verification 
	/// failures.
	/// </devdoc>
	internal class MockVerificationException : MockException
	{
		Type targetType;
		List<Expression> failedSetups;

		public MockVerificationException(Type targetType, List<Expression> failedSetups)
			: base(ExceptionReason.VerificationFailed, GetMessage(targetType, failedSetups))
		{
			this.targetType = targetType;
			this.failedSetups = failedSetups;
		}

		private static string GetMessage(Type targetType, List<Expression> failedSetups)
		{
			return String.Format(Properties.Resources.VerficationFailed, GetRawSetups(targetType, failedSetups));
		}

		private static string GetRawSetups(Type targetType, List<Expression> failedSetups)
		{
			StringBuilder message = new StringBuilder();
			string targetTypeName = targetType.Name;
			foreach (var expr in failedSetups)
			{
				message
					.Append(targetTypeName)
					.Append(" ")
					.AppendLine(expr.ToStringFixed());
			}

			return message.ToString();
		}

		internal string GetRawSetups()
		{
			return GetRawSetups(targetType, failedSetups);
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		protected MockVerificationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}

}
