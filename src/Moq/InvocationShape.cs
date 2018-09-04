//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.
//
//Redistribution and use in source and binary forms,
//with or without modification, are permitted provided
//that the following conditions are met:
//
//    * Redistributions of source code must retain the
//    above copyright notice, this list of conditions and
//    the following disclaimer.
//
//    * Redistributions in binary form must reproduce
//    the above copyright notice, this list of conditions
//    and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the
//    names of its contributors may be used to endorse
//    or promote products derived from this software
//    without specific prior written permission.
//
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
//
//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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
