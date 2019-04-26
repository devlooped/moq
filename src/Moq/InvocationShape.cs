// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private Expression[] partiallyEvaluatedArguments;

		private readonly Dictionary<TypeInfo, bool> explicitMappingCache;

		public InvocationShape(LambdaExpression expression, MethodInfo method, IReadOnlyList<Expression> arguments = null)
		{
			Debug.Assert(expression != null);
			Debug.Assert(method != null);

			Guard.IsOverridable(method, expression);
			Guard.IsVisibleToProxyFactory(method);

			this.Expression = expression;
			this.Method = method;
			this.explicitMappingCache = new Dictionary<TypeInfo, bool>();
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
			var arguments = invocation.Arguments;
			if (this.argumentMatchers.Length != arguments.Length)
			{
				return false;
			}

			if (invocation.Method != this.Method && !this.IsOverride(invocation.Method))
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
			var method = this.Method;

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

			if (method.IsGenericMethod || invocationMethod.IsGenericMethod)
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
			
			if (IsExplicitlyImplementedBy(invocationMethod.DeclaringType.GetTypeInfo()))
			{
				return false;
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

			for (int i = 0, n = this.partiallyEvaluatedArguments.Length; i < n; ++i)
			{
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

		private bool IsExplicitlyImplementedBy(TypeInfo typeInfo)
		{
			if (!this.explicitMappingCache.TryGetValue(typeInfo, out var isExplicit))
			{
				var methodTypeInfo = this.Method.DeclaringType.GetTypeInfo();

				if (!methodTypeInfo.IsInterface || typeInfo.IsInterface)
				{
					isExplicit = false;
					this.explicitMappingCache[typeInfo] = isExplicit;
				}
				else
				{
					var map = typeInfo.GetRuntimeInterfaceMap(methodTypeInfo);
					var index = Array.IndexOf(map.InterfaceMethods, this.Method);
					isExplicit = map.TargetMethods[index].IsPrivate;
					this.explicitMappingCache[typeInfo] = isExplicit;
				}
			}

			return isExplicit;
		}
	}
}
