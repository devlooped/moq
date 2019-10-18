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
	///   <para>
	///     This shape is described by <see cref="InvocationShape.Expression"/> which has the general form
	///     `mock => mock.Method(...arguments)`. Because the method and arguments are frequently needed,
	///     they are cached in <see cref="InvocationShape.Method"/> and <see cref="InvocationShape.Arguments"/>
	///     for faster access.
	///   </para>
	/// </summary>
	internal sealed class InvocationShape : IEquatable<InvocationShape>
	{
		private static readonly Expression[] noArguments = new Expression[0];
		private static readonly IMatcher[] noArgumentMatchers = new IMatcher[0];

		public readonly LambdaExpression Expression;
		public readonly MethodInfo Method;
		public readonly IReadOnlyList<Expression> Arguments;

		private readonly IMatcher[] argumentMatchers;
		private MethodInfo methodImplementation;
		private Expression[] partiallyEvaluatedArguments;
#if DEBUG
		private Type proxyType;
#endif

		public InvocationShape(LambdaExpression expression, MethodInfo method, IReadOnlyList<Expression> arguments = null)
		{
			Debug.Assert(expression != null);
			Debug.Assert(method != null);

			Guard.IsOverridable(method, expression);
			Guard.IsVisibleToProxyFactory(method);

			this.Expression = expression;
			this.Method = method;
			if (arguments != null)
			{
				(this.argumentMatchers, this.Arguments) = MatcherFactory.CreateMatchers(arguments, method.GetParameters());
			}
			else
			{
				this.argumentMatchers = noArgumentMatchers;
				this.Arguments = noArguments;
			}
		}

		public void Deconstruct(out LambdaExpression expression, out MethodInfo method, out IReadOnlyList<Expression> arguments)
		{
			expression = this.Expression;
			method = this.Method;
			arguments = this.Arguments;
		}

		public bool IsMatch(Invocation invocation)
		{
			if (invocation.Method != this.Method && !this.IsOverride(invocation))
			{
				return false;
			}

			var arguments = invocation.Arguments;
			var parameterTypes = invocation.Method.GetParameterTypes();
			for (int i = 0, n = this.argumentMatchers.Length; i < n; ++i)
			{
				if (this.argumentMatchers[i].Matches(arguments[i], parameterTypes[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		public void SetupEvaluatedSuccessfully(Invocation invocation)
		{
			var arguments = invocation.Arguments;
			var parameterTypes = invocation.Method.GetParameterTypes();
			for (int i = 0, n = this.argumentMatchers.Length; i < n; ++i)
			{
				this.argumentMatchers[i].SetupEvaluatedSuccessfully(arguments[i], parameterTypes[i]);
			}
		}

		private bool IsOverride(Invocation invocation)
		{
			Debug.Assert(invocation.Method != this.Method);

			var method = this.Method;
			var invocationMethod = invocation.Method;

			var proxyType = invocation.ProxyType;
#if DEBUG
			// The following `if` block is a sanity check to ensure this `InvocationShape` always
			// runs against the same proxy type. This is important because we're caching the result
			// of mapping methods into that particular proxy type. We have no cache invalidation
			// logic in place; instead, we simply assume that the cached results will stay valid.
			// If the below assertion fails, that assumption was wrong.
			if (this.proxyType == null)
			{
				this.proxyType = proxyType;
			}
			else
			{
				Debug.Assert(this.proxyType == proxyType);
			}
#endif

			// If not already in the cache, map this `InvocationShape`'s method into the proxy type:
			if (this.methodImplementation == null)
			{
				this.methodImplementation = method.GetImplementingMethod(proxyType);
			}

			if (invocation.MethodImplementation != this.methodImplementation)
			{
				return false;
			}

			if (method.IsGenericMethod || invocationMethod.IsGenericMethod)
			{
				if (!method.GetGenericArguments().CompareTo(invocationMethod.GetGenericArguments(), TypeComparison.TypeMatchersOrElseAssignmentCompatibility))
				{
					return false;
				}
			}

			return true;
		}

		public bool Equals(InvocationShape other)
		{
			if (this.Method != other.Method)
			{
				return false;
			}

			if (this.Arguments.Count != other.Arguments.Count)
			{
				return false;
			}

			if (this.partiallyEvaluatedArguments == null)
			{
				this.partiallyEvaluatedArguments = PartiallyEvaluateArguments(this.Arguments);
			}

			if (other.partiallyEvaluatedArguments == null)
			{
				other.partiallyEvaluatedArguments = PartiallyEvaluateArguments(other.Arguments);
			}

			var lastParameter = this.Method.GetParameters().LastOrDefault();
			var lastParameterIsParamArray = lastParameter != null && lastParameter.ParameterType.IsArray && lastParameter.IsDefined(typeof(ParamArrayAttribute));

			for (int i = 0, li = this.partiallyEvaluatedArguments.Length - 1; i <= li; ++i)
			{
				// Special case for final `params` parameters, which need to be compared by structural equality,
				// not array reference equality:
				if (i == li && lastParameterIsParamArray)
				{
					if (this.Arguments[li] is NewArrayExpression e1 && other.Arguments[li] is NewArrayExpression e2 && e1.Expressions.Count == e2.Expressions.Count)
					{
						for (int j = 0, nj = e1.Expressions.Count; j < nj; ++j)
						{
							if (!ExpressionComparer.Default.Equals(e1.Expressions[j], e2.Expressions[j]))
							{
								return false;
							}
						}

						continue;
					}
				}

				if (!ExpressionComparer.Default.Equals(this.partiallyEvaluatedArguments[i], other.partiallyEvaluatedArguments[i]))
				{
					return false;
				}
			}

			return true;
		}

		private static Expression[] PartiallyEvaluateArguments(IReadOnlyList<Expression> arguments)
		{
			Debug.Assert(arguments != null);

			if (arguments.Count == 0)
			{
				return noArguments;
			}

			var partiallyEvaluatedArguments = new Expression[arguments.Count];
			for (int i = 0, n = arguments.Count; i < n; ++i)
			{
				partiallyEvaluatedArguments[i] = arguments[i].PartialMatcherAwareEval();
			}

			return partiallyEvaluatedArguments;
		}

		public override bool Equals(object obj)
		{
			return obj is InvocationShape other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.Method.GetHashCode();
		}

		public override string ToString()
		{
			return this.Expression.ToStringFixed();
		}
	}
}
