// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Moq.Async;

namespace Moq
{
	internal abstract class Invocation : IInvocation
	{
		private object[] arguments;
		private MethodInfo method;
		private MethodInfo methodImplementation;
		private readonly Type proxyType;
		private object result;
		private ISetup matchingSetup;
		private bool verified;

		/// <summary>
		/// Initializes a new instance of the <see cref="Invocation"/> class.
		/// </summary>
		/// <param name="proxyType">The <see cref="Type"/> of the concrete proxy object on which a method is being invoked.</param>
		/// <param name="method">The method being invoked.</param>
		/// <param name="arguments">The arguments with which the specified <paramref name="method"/> is being invoked.</param>
		protected Invocation(Type proxyType, MethodInfo method, params object[] arguments)
		{
			Debug.Assert(proxyType != null);
			Debug.Assert(arguments != null);
			Debug.Assert(method != null);

			this.arguments = arguments;
			this.method = method;
			this.proxyType = proxyType;
		}

		/// <summary>
		/// Gets the method of the invocation.
		/// </summary>
		public MethodInfo Method => this.method;

		public MethodInfo MethodImplementation
		{
			get
			{
				if (this.methodImplementation == null)
				{
					this.methodImplementation = this.method.GetImplementingMethod(this.proxyType);
				}

				return this.methodImplementation;
			}
		}

		/// <summary>
		/// Gets the arguments of the invocation.
		/// </summary>
		/// <remarks>
		/// Arguments may be modified. Derived classes must ensure that by-reference parameters are written back
		/// when the invocation is ended by a call to any of the three <c>Returns</c> methods.
		/// </remarks>
		public object[] Arguments => this.arguments;

		public ISetup MatchingSetup => this.matchingSetup;

		public Type ProxyType => this.proxyType;

		public object ReturnValue
		{
			get => this.result is ExceptionResult ? null : this.result;
			set
			{
				Debug.Assert(this.result == null);
				this.result = value;
			}
		}

		public Exception Exception
		{
			get => this.result is ExceptionResult r ? r.Exception : null;
			set
			{
				Debug.Assert(this.result == null);
				this.result = new ExceptionResult(value);
			}
		}

		public void ConvertResultToAwaitable(IAwaitableFactory awaitableFactory)
		{
			if (this.result is ExceptionResult r)
			{
				this.result = awaitableFactory.CreateFaulted(r.Exception);
			}
			else if (this.result != null && !this.method.ReturnType.IsAssignableFrom(this.result.GetType()))
			{
				this.result = awaitableFactory.CreateCompleted(this.result);
			}
		}

		public bool IsVerified => this.verified;

		/// <summary>
		///   Calls the <see langword="base"/> method implementation
		///   and returns its return value (or <see langword="null"/> for <see langword="void"/> methods).
		/// </summary>
		public abstract object CallBase();

		public void MarkAsMatchedBy(ISetup setup)
		{
			Debug.Assert(this.matchingSetup == null);

			this.matchingSetup = setup;
		}

		internal void MarkAsVerified() => this.verified = true;

		internal void MarkAsVerifiedIfMatchedBy(Func<ISetup, bool> predicate)
		{
			if (this.matchingSetup != null && predicate(this.matchingSetup))
			{
				this.verified = true;
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			var method = this.Method;

			var builder = new StringBuilder();
			builder.AppendNameOf(method.DeclaringType);
			builder.Append('.');

			if (method.IsGetAccessor())
			{
				builder.Append(method.Name, 4, method.Name.Length - 4);
			}
			else if (method.IsSetAccessor())
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

		/// <summary>
		/// Internal type to mark invocation results as "exception occurred during execution". The type just
		/// wraps the Exception so a thrown exception can be distinguished from an <see cref="System.Exception"/>
		/// return value.
		/// </summary>
		private readonly struct ExceptionResult
		{
			public ExceptionResult(Exception exception)
			{
				Exception = exception;
			}

			public Exception Exception { get; }
		}
	}
}
