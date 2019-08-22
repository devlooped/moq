// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Matchers;
using Moq.Properties;

namespace Moq
{
	internal static class MatcherFactory
	{
		public static Pair<IMatcher[], Expression[]> CreateMatchers(IReadOnlyList<Expression> arguments, ParameterInfo[] parameters)
		{
			Debug.Assert(arguments != null);
			Debug.Assert(parameters != null);
			Debug.Assert(arguments.Count == parameters.Length);

			var n = parameters.Length;
			var evaluatedArguments = new Expression[n];
			var argumentMatchers = new IMatcher[n];
			for (int i = 0; i < n; ++i)
			{
				(argumentMatchers[i], evaluatedArguments[i]) = MatcherFactory.CreateMatcher(arguments[i], parameters[i]);
			}
			return new Pair<IMatcher[], Expression[]>(argumentMatchers, evaluatedArguments);
		}

		public static Pair<IMatcher, Expression> CreateMatcher(Expression argument, ParameterInfo parameter)
		{
			if (parameter.ParameterType.IsByRef)
			{
				if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out)
				{
					// `out` parameter
					return new Pair<IMatcher, Expression>(AnyMatcher.Instance, argument);
				}
				else
				{
					// `ref` parameter

					// Test for special case: `It.Ref<TValue>.IsAny`
					if (argument is MemberExpression memberExpression)
					{
						var member = memberExpression.Member;
						if (member.Name == nameof(It.Ref<object>.IsAny))
						{
							var memberDeclaringType = member.DeclaringType;
							if (memberDeclaringType.IsGenericType)
							{
								var memberDeclaringTypeDefinition = memberDeclaringType.GetGenericTypeDefinition();
								if (memberDeclaringTypeDefinition == typeof(It.Ref<>))
								{
									return new Pair<IMatcher, Expression>(AnyMatcher.Instance, argument);
								}
							}
						}
					}

					if (argument.PartialEval() is ConstantExpression constant)
					{
						return new Pair<IMatcher, Expression>(new RefMatcher(constant.Value), constant);
					}

					throw new NotSupportedException(Resources.RefExpressionMustBeConstantValue);
				}
			}
			else if (parameter.IsDefined(typeof(ParamArrayAttribute), true) && argument.NodeType == ExpressionType.NewArrayInit)
			{
				var newArrayExpression = (NewArrayExpression)argument;

				Debug.Assert(newArrayExpression.Type.IsArray);
				var elementType = newArrayExpression.Type.GetElementType();

				var n = newArrayExpression.Expressions.Count;
				var matchers = new IMatcher[n];
				var initializers = new Expression[n];

				for (int i = 0; i < n; ++i)
				{
					(matchers[i], initializers[i]) = MatcherFactory.CreateMatcher(newArrayExpression.Expressions[i]);
					initializers[i] = initializers[i].ConvertIfNeeded(elementType);
				}
				return new Pair<IMatcher, Expression>(new ParamArrayMatcher(matchers), Expression.NewArrayInit(elementType, initializers));
			}
			else
			{
				return MatcherFactory.CreateMatcher(argument);
			}
		}

		public static Pair<IMatcher, Expression> CreateMatcher(Expression expression)
		{
			// Type inference on the call might 
			// do automatic conversion to the desired 
			// method argument type, and a Convert expression type 
			// might be the topmost instead.
			// i.e.: It.IsInRange(0, 100, Range.Inclusive)
			// the values are ints, but if the method to call 
			// expects, say, a double, a Convert node will be on 
			// the expression.
			var originalExpression = expression;
			if (expression.NodeType == ExpressionType.Convert)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			// SetupSet passes a custom expression.
			if (expression is MatchExpression matchExpression)
			{
				return new Pair<IMatcher, Expression>(matchExpression.Match, matchExpression);
			}

			if (expression is MethodCallExpression call)
			{
				if (expression.IsMatch(out var match))
				{
					return new Pair<IMatcher, Expression>(match, expression);
				}

#pragma warning disable 618
				if (call.Method.IsDefined(typeof(MatcherAttribute), true))
				{
					return new Pair<IMatcher, Expression>(new MatcherAttributeMatcher(call), call);
				}
#pragma warning restore 618

				var method = call.Method;
				if (!method.IsGetAccessor())
				{
					return new Pair<IMatcher, Expression>(new LazyEvalMatcher(originalExpression), originalExpression);
				}
			}
			else if (expression is MemberExpression || expression is IndexExpression)
			{
				if (expression.IsMatch(out var match))
				{
					return new Pair<IMatcher, Expression>(match, expression);
				}
			}

			// Try reducing locals to get a constant.
			var reduced = originalExpression.PartialEval();
			if (reduced.NodeType == ExpressionType.Constant)
			{
				return new Pair<IMatcher, Expression>(new ConstantMatcher(((ConstantExpression)reduced).Value), reduced);
			}

			if (reduced.NodeType == ExpressionType.Quote)
			{
				return new Pair<IMatcher, Expression>(new ExpressionMatcher(((UnaryExpression)expression).Operand), reduced);
			}

			throw new NotSupportedException(
				string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedExpression, expression));
		}
	}
}
