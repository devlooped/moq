// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal abstract class Invocation : IInvocation
	{
		private object[] arguments;
		private MethodInfo method;
		private VerificationState verificationState;

		/// <summary>
		/// Initializes a new instance of the <see cref="Invocation"/> class.
		/// </summary>
		/// <param name="method">The method being invoked.</param>
		/// <param name="arguments">The arguments with which the specified <paramref name="method"/> is being invoked.</param>
		protected Invocation(MethodInfo method, params object[] arguments)
		{
			Debug.Assert(arguments != null);
			Debug.Assert(method != null);

			this.arguments = arguments;
			this.method = method;
		}

		/// <summary>
		/// Gets the method of the invocation.
		/// </summary>
		public MethodInfo Method => this.method;

		/// <summary>
		/// Gets the arguments of the invocation.
		/// </summary>
		/// <remarks>
		/// Arguments may be modified. Derived classes must ensure that by-reference parameters are written back
		/// when the invocation is ended by a call to any of the three <c>Returns</c> methods.
		/// </remarks>
		public object[] Arguments => this.arguments;

		IReadOnlyList<object> IInvocation.Arguments => this.arguments;

		internal bool Verified => this.verificationState == VerificationState.Verified;

		/// <summary>
		/// Ends the invocation as if a <see langword="return"/> statement occurred.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a <see langword="void"/> method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void Return();

		/// <summary>
		/// Ends the invocation as if a tail call to the base method were made.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a method having a callable (non-<see langword="abstract"/>) base method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void ReturnBase();

		/// <summary>
		/// Ends the invocation as if a <see langword="return"/> statement with the specified return value occurred.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a non-<see langword="void"/> method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void Return(object value);

		internal void MarkAsMatchedBySetup()  // this supports the `mock.VerifyAll()` machinery
		{
			if (this.verificationState == VerificationState.Invoked)
			{
				this.verificationState = VerificationState.InvokedAndMatchedBySetup;
			}
		}

		internal void MarkAsMatchedByVerifiableSetup()  // this supports the `mock.Verify()` machinery
		{
			if (this.verificationState == VerificationState.Invoked ||
				this.verificationState == VerificationState.InvokedAndMatchedBySetup)
			{
				this.verificationState = VerificationState.InvokedAndMatchedByVerifiableSetup;
			}
		}

		internal void MarkAsVerified() => this.verificationState = VerificationState.Verified;

		internal void MarkAsVerifiedIfMatchedBySetup()  // this supports the `mock.VerifyAll()` machinery
		{
			if (this.verificationState == VerificationState.InvokedAndMatchedBySetup ||
				this.verificationState == VerificationState.InvokedAndMatchedByVerifiableSetup)
			{
				this.verificationState = VerificationState.Verified;
			}
		}

		internal void MarkAsVerifiedIfMatchedByVerifiableSetup()  // this supports the `mock.Verify()` machinery
		{
			if (this.verificationState == VerificationState.InvokedAndMatchedByVerifiableSetup)
			{
				this.verificationState = VerificationState.Verified;
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			var method = this.Method;

			var builder = new StringBuilder();
			builder.Append(method.DeclaringType.Name);
			builder.Append('.');

			if (method.IsPropertyGetter())
			{
				builder.Append(method.Name, 4, method.Name.Length - 4);
			}
			else if (method.IsPropertySetter())
			{
				builder.Append(method.Name, 4, method.Name.Length - 4);
				builder.Append(" = ");
				builder.AppendValueOf(this.Arguments[0]);
			}
			else
			{
				builder.AppendNameOf(method, includeGenericArgumentList: true);

				// append argument list:
				builder.Append('(');
				for (int i = 0, n = this.Arguments.Length; i < n; ++i)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.AppendValueOf(this.Arguments[i]);
				}

				builder.Append(')');
			}

			return builder.ToString();
		}

		private enum VerificationState : byte
		{
			Invoked = 0,
			InvokedAndMatchedBySetup,
			InvokedAndMatchedByVerifiableSetup,
			Verified,
		}
	}
}
