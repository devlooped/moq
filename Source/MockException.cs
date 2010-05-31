//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using Moq.Properties;
using Moq.Proxy;

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
	[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "It's only initialized internally.")]
#if !SILVERLIGHT
	[Serializable]
#endif
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

		private ExceptionReason reason;

		internal MockException(ExceptionReason reason, MockBehavior behavior,
			ICallContext invocation)
			: this(reason, behavior, invocation,
				Properties.Resources.ResourceManager.GetString(reason.ToString()))
		{
		}

		internal MockException(ExceptionReason reason, MockBehavior behavior,
			ICallContext invocation, string message)
			: base(GetMessage(behavior, invocation, message))
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

		private static string GetMessage(
			MockBehavior behavior,
			ICallContext invocation,
			string message)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Resources.MockExceptionMessage,
				invocation.Format(),
				behavior,
				message
			);
		}

#if !SILVERLIGHT
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
		[SecurityCritical]
		[SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", reason);
		}
#endif
	}

	/// <devdoc>
	/// Used by the mock factory to accumulate verification 
	/// failures.
	/// </devdoc>
#if !SILVERLIGHT
	[Serializable]
#endif
	internal class MockVerificationException : MockException
	{
		IProxyCall[] failedSetups;

		public MockVerificationException(IProxyCall[] failedSetups)
			: base(ExceptionReason.VerificationFailed, GetMessage(failedSetups))
		{
			this.failedSetups = failedSetups;
		}

		private static string GetMessage(IProxyCall[] failedSetups)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Resources.VerficationFailed,
				GetRawSetups(failedSetups));
		}

		private static string GetRawSetups(IProxyCall[] failedSetups)
		{
			return failedSetups.Aggregate(string.Empty, (s, call) => s + call.ToString() + Environment.NewLine);

			//var message = new StringBuilder();
			//foreach (var setup in failedSetups)
			//{
			//   if (setup.FailMessage != null)
			//   {
			//      message.Append(setup.FailMessage).Append(": ");
			//   }

			//   var lambda = setup.SetupExpression.PartialMatcherAwareEval().ToLambda();
			//   var targetTypeName = lambda.Parameters[0].Type.Name;

			//   message.Append(targetTypeName).Append(" ").Append(lambda.ToStringFixed());

			//   if (setup.TestMethod != null)
			//   {
			//      message.AppendFormat(
			//         " ({0}() in {1}: line {2})",
			//         setup.TestMethod.Name,
			//         setup.FileName,
			//         setup.FileLine);
			//   }

			//   message.AppendLine();
			//}

			//return message.ToString();
		}

		internal string GetRawSetups()
		{
			return GetRawSetups(failedSetups);
		}

#if !SILVERLIGHT
		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		protected MockVerificationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}