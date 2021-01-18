// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Matchers
{
	/// <summary>
	/// Matcher to treat static functions as matchers.
	/// 
	/// mock.Setup(x => x.StringMethod(A.MagicString()));
	/// 
	/// public static class A 
	/// {
	///     [Matcher]
	///     public static string MagicString() { return null; }
	///     public static bool MagicString(string arg)
	///     {
	///         return arg == "magic";
	///     }
	/// }
	/// 
	/// Will succeed if: mock.Object.StringMethod("magic");
	/// and fail with any other call.
	/// </summary>
	internal class MatcherAttributeMatcher : IMatcher
	{
		private MethodInfo validatorMethod;
		private MethodCallExpression expression;

		public MatcherAttributeMatcher(MethodCallExpression expression)
		{
			this.validatorMethod = ResolveValidatorMethod(expression);
			this.expression = expression;
		}

		private static MethodInfo ResolveValidatorMethod(MethodCallExpression call)
		{
			var expectedParametersTypes = new[] { call.Method.ReturnType }.Concat(call.Method.GetParameters().Select(p => p.ParameterType)).ToArray();

			MethodInfo method = null;

			if (call.Method.IsGenericMethod)
			{
				// This is the "hard" way in .NET 3.5 as GetMethod does not support
				// passing generic type arguments for the query.
				var genericArgs = call.Method.GetGenericArguments();

				method = call.Method.DeclaringType.GetMethods(call.Method.Name)
					.Where(m =>
						m.IsGenericMethodDefinition &&
						m.GetGenericArguments().Length ==
							call.Method.GetGenericMethodDefinition().GetGenericArguments().Length &&
						expectedParametersTypes.SequenceEqual(
							m.MakeGenericMethod(genericArgs).GetParameters().Select(p => p.ParameterType)))
					.Select(m => m.MakeGenericMethod(genericArgs))
					.FirstOrDefault();
			}
			else
			{
				method = call.Method.DeclaringType.GetMethod(call.Method.Name, expectedParametersTypes);
			}

			// throw if validatorMethod doesn't exists			
			if (method == null)
			{
				throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture,
					"public {0}bool {1}({2}) in class {3}.",
					call.Method.IsStatic ? "static " : String.Empty,
					call.Method.Name,
					String.Join(", ", expectedParametersTypes.Select(x => x.Name).ToArray()),
					call.Method.DeclaringType.ToString()));
			}
			return method;
		}

		public bool Matches(object argument, Type parameterType)
		{
			// use matcher Expression to get extra arguments
			var extraArgs = this.expression.Arguments.Select(ae => ((ConstantExpression)ae.PartialEval()).Value);
			var args = new[] { argument }.Concat(extraArgs).ToArray();
			// for static and non-static method
			var instance = this.expression.Object == null ? null : (this.expression.Object.PartialEval() as ConstantExpression).Value;
			return (bool)validatorMethod.Invoke(instance, args);
		}

		public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
		}
	}
}
