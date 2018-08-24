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
				var call = (MethodCallExpression)expression;

				// Try to determine if invocation is to a matcher.
				using (var context = new FluentMockContext())
				{
					Expression.Lambda<Action>(call).CompileUsingExpressionCompiler().Invoke();

					if (context.LastMatch != null)
					{
						return context.LastMatch;
					}
				}

#pragma warning disable 618
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
				// Try to determine if invocation is to a matcher.
				using (var context = new FluentMockContext())
				{
					Expression.Lambda<Action>((MemberExpression)expression).CompileUsingExpressionCompiler().Invoke();
					if (context.LastMatch != null)
					{
						return context.LastMatch;
					}
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
