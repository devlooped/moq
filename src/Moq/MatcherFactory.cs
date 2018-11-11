// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using System.Linq.Expressions;
using Moq.Properties;
using Moq.Matchers;
using System.Reflection;

namespace Moq
{
	internal static class MatcherFactory
	{
		public static IMatcher CreateMatcher(Expression argument, ParameterInfo parameter)
		{
			if (parameter.ParameterType.IsByRef)
			{
				if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out)
				{
					// `out` parameter
					return AnyMatcher.Instance;
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
							if (memberDeclaringType.GetTypeInfo().IsGenericType)
							{
								var memberDeclaringTypeDefinition = memberDeclaringType.GetGenericTypeDefinition();
								if (memberDeclaringTypeDefinition == typeof(It.Ref<>))
								{
									return AnyMatcher.Instance;
								}
							}
						}
					}

					var constant = argument.PartialEval() as ConstantExpression;
					if (constant == null)
					{
						throw new NotSupportedException(Resources.RefExpressionMustBeConstantValue);
					}

					return new RefMatcher(constant.Value);
				}
			}
			else if (parameter.IsDefined(typeof(ParamArrayAttribute), true) && (argument.NodeType == ExpressionType.NewArrayInit || !argument.Type.IsArray))
			{
				return new ParamArrayMatcher((NewArrayExpression)argument);
			}
			else
			{
				return MatcherFactory.CreateMatcher(argument);
			}
		}

		public static IMatcher CreateMatcher(Expression expression)
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
			var matchExpression = expression as MatchExpression;
			if (matchExpression != null)
			{
				return matchExpression.Match;
			}

			if (expression.NodeType == ExpressionType.Call)
			{
				if (expression.IsMatch(out var match))
				{
					return match;
				}

#pragma warning disable 618
				var call = (MethodCallExpression)expression;
				if (call.Method.IsDefined(typeof(MatcherAttribute), true))
				{
					return new MatcherAttributeMatcher(call);
				}
#pragma warning restore 618
				else
				{
					return new LazyEvalMatcher(originalExpression);
				}
			}
			else if (expression.NodeType == ExpressionType.MemberAccess)
			{
				if (expression.IsMatch(out var match))
				{
					return match;
				}
			}

			// Try reducing locals to get a constant.
			var reduced = originalExpression.PartialEval();
			if (reduced.NodeType == ExpressionType.Constant)
			{
				return new ConstantMatcher(((ConstantExpression)reduced).Value);
			}

			if (reduced.NodeType == ExpressionType.Quote)
			{
				return new ExpressionMatcher(((UnaryExpression)expression).Operand);
			}

			throw new NotSupportedException(
				string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedExpression, expression));
		}
	}
}
