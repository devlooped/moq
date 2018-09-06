// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Describes the "shape" of an invocation against which concrete <see cref="Invocation"/>s can be matched.
	/// </summary>
	internal readonly struct InvocationShape
	{
		private readonly MethodInfo method;
		private readonly IMatcher[] argumentMatchers;

		public InvocationShape(MethodInfo method, IReadOnlyList<Expression> arguments)
		{
			this.method = method;
			this.argumentMatchers = GetArgumentMatchers(arguments, method.GetParameters());
		}

		public InvocationShape(MethodInfo method, IMatcher[] argumentMatchers)
		{
			this.method = method;
			this.argumentMatchers = argumentMatchers;
		}

		public IReadOnlyList<IMatcher> ArgumentMatchers => this.argumentMatchers;

		public MethodInfo Method => this.method;

		public bool IsMatch(Invocation invocation)
		{
			var arguments = invocation.Arguments;
			if (this.argumentMatchers.Length != arguments.Length)
			{
				return false;
			}

			if (invocation.Method != this.method && !this.IsOverride(invocation.Method))
			{
				return false;
			}

			for (int i = 0, n = this.argumentMatchers.Length; i < n; ++i)
			{
				if (this.argumentMatchers[i].Matches(arguments[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		private bool IsOverride(MethodInfo invocationMethod)
		{
			var method = this.method;

			if (!method.DeclaringType.IsAssignableFrom(invocationMethod.DeclaringType))
			{
				return false;
			}

			if (!method.Name.Equals(invocationMethod.Name, StringComparison.Ordinal))
			{
				return false;
			}

			if (method.ReturnType != invocationMethod.ReturnType)
			{
				return false;
			}

			if (method.IsGenericMethod)
			{
				if (!method.GetGenericArguments().CompareTo(invocationMethod.GetGenericArguments(), exact: false))
				{
					return false;
				}
			}
			else
			{
				if (!invocationMethod.GetParameterTypes().CompareTo(method.GetParameterTypes(), exact: true))
				{
					return false;
				}
			}

			return true;
		}

		private static IMatcher[] GetArgumentMatchers(IReadOnlyList<Expression> arguments, ParameterInfo[] parameters)
		{
			Debug.Assert(arguments != null);
			Debug.Assert(parameters != null);
			Debug.Assert(arguments.Count == parameters.Length);

			var n = parameters.Length;
			var argumentMatchers = new IMatcher[n];
			for (int i = 0; i < n; ++i)
			{
				argumentMatchers[i] = MatcherFactory.CreateMatcher(arguments[i], parameters[i]);
			}
			return argumentMatchers;
		}
	}
}
