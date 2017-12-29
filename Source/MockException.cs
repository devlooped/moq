//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif
using System.Security;

using Moq.Diagnostics.Errors;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Exception thrown by mocks when they are not properly set up,
	/// when setups are not matched, when verification fails, etc.
	/// </summary>
	/// <remarks>
	/// A distinct exception type is provided so that exceptions
	/// thrown by a mock can be distinguished from other exceptions
	/// that might be thrown in tests.
	/// <para>
	/// Moq does not provide a richer hierarchy of exception types, as
	/// tests typically should <em>not</em> catch or expect exceptions
	/// from mocks. These are typically the result of changes
	/// in the tested class or its collaborators' implementation, and
	/// result in fixes in the mock setup so that they disappear and
	/// allow the test to pass.
	/// </para>
	/// </remarks>
	[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "It's only initialized internally.")]
#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class MockException : Exception
	{
		// Made internal as it's of no use for consumers, but it's important for our own tests.
		internal enum ExceptionReason
		{
			NoSetup,
			ReturnValueRequired,
			VerificationFailed,
			MoreThanOneCall,
			MoreThanNCalls,
		}

#if FEATURE_SERIALIZATION
		[NonSerialized]
#endif
		private IError error;

		private ExceptionReason reason;

		internal MockException(ExceptionReason reason, MockBehavior behavior, Invocation invocation)
			: this(reason, behavior, invocation, Resources.ResourceManager.GetString(reason.ToString()))
		{
		}

		internal MockException(ExceptionReason reason, MockBehavior behavior, Invocation invocation, string message)
			: base(GetMessage(behavior, invocation, message))
		{
			this.reason = reason;
		}

		internal MockException(ExceptionReason reason, string exceptionMessage)
			: base(exceptionMessage)
		{
			this.reason = reason;
		}

		internal MockException(ExceptionReason reason, IError error)
			: base(error.Message)
		{
			Debug.Assert(error != null);

			this.error = error;
			this.reason = reason;
		}

		internal IError Error => this.error;

		internal ExceptionReason Reason
		{
			get { return reason; }
		}

		/// <summary>
		/// Indicates whether this exception is a verification fault raised by Verify()
		/// </summary>
		public bool IsVerificationError
		{
			get { return reason == ExceptionReason.VerificationFailed; }
		}

		private static string GetMessage(MockBehavior behavior, Invocation invocation, string message)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Resources.MockExceptionMessage,
				invocation.ToString(),
				behavior,
				message
			);
		}

#if FEATURE_SERIALIZATION
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
}
