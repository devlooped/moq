// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

namespace Moq
{
	internal static class ExpressionExtensions
	{
		internal static Delegate CompileUsingExpressionCompiler(this LambdaExpression expression)
		{
			// Expression trees are not compiled directly.
			// The indirection via an ExpressionCompiler allows users to plug a different expression compiler.
			return ExpressionCompiler.Instance.Compile(expression);
		}

		internal static TDelegate CompileUsingExpressionCompiler<TDelegate>(this Expression<TDelegate> expression) where TDelegate : Delegate
		{
			// Expression trees are not compiled directly.
			// The indirection via an ExpressionCompiler allows users to plug a different expression compiler.
			return ExpressionCompiler.Instance.Compile(expression);
		}

		/// <summary>
		/// Converts the body of the lambda expression into the <see cref="PropertyInfo"/> referenced by it.
		/// </summary>
		public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
		{
			if (expression.Body is MemberExpression prop)
			{
				if (prop.Member is PropertyInfo info)
				{
					// the following block is required because .NET compilers put the wrong PropertyInfo into MemberExpression
					// for properties originally declared in base classes; they will put the base class' PropertyInfo into
					// the expression. we attempt to correct this here by checking whether the type of the accessed object
					// has a property by the same name whose base definition equals the property in the expression; if so,
					// we "upgrade" to the derived property.
					if (info.DeclaringType != prop.Expression.Type && info.CanRead)
					{
						var propertyInLeft = prop.Expression.Type.GetProperty(info.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (propertyInLeft != null && propertyInLeft.GetMethod.GetBaseDefinition() == info.GetMethod)
						{
							info = propertyInLeft;
						}
					}

					return info;
				}
			}

			throw new ArgumentException(string.Format(
				CultureInfo.CurrentCulture,
				Resources.SetupNotProperty,
				expression.ToStringFixed()));
		}

		/// <summary>
		/// Checks whether the body of the lambda expression is a property access.
		/// </summary>
		public static bool IsProperty(this LambdaExpression expression)
		{
			Guard.NotNull(expression, nameof(expression));

			return expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo;
		}

		/// <summary>
		/// Checks whether the body of the lambda expression is a property indexer, which is true 
		/// when the expression is an <see cref="MethodCallExpression"/> whose 
		/// <see cref="MethodCallExpression.Method"/> has <see cref="MethodBase.IsSpecialName"/> 
		/// equal to <see langword="true"/>.
		/// </summary>
		public static bool IsPropertyIndexer(this LambdaExpression expression)
		{
			Guard.NotNull(expression, nameof(expression));

			return expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.IsSpecialName;
		}

		public static Expression StripQuotes(this Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			return expression;
		}

		public static Expression PartialEval(this Expression expression)
		{
			return Evaluator.PartialEval(expression);
		}

		public static LambdaExpression PartialMatcherAwareEval(this LambdaExpression expression)
		{
			return (LambdaExpression)Evaluator.PartialEval(
				expression,
				PartialMatcherAwareEval_ShouldEvaluate);
		}

		private static bool PartialMatcherAwareEval_ShouldEvaluate(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Parameter:
					return false;

				case ExpressionType.Call:
				case ExpressionType.MemberAccess:
					// Evaluate everything but matchers:
					using (var context = new FluentMockContext())
					{
						Expression.Lambda<Action>(expression).CompileUsingExpressionCompiler().Invoke();
						return context.LastMatch == null;
					}

				default:
					return true;
			}
		}

		/// <devdoc>
		/// TODO: remove this code when https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=331583 
		/// is fixed.
		/// </devdoc>
		public static string ToStringFixed(this Expression expression)
		{
			return new ExpressionStringBuilder().Append(expression).ToString();
		}

		/// <summary>
		/// Extracts, into a common form, information from a <see cref="LambdaExpression" />
		/// around either a <see cref="MethodCallExpression" /> (for a normal method call)
		/// or a <see cref="InvocationExpression" /> (for a delegate invocation).
		/// </summary>
		internal static (Expression Object, MethodInfo Method, IReadOnlyList<Expression> Arguments) GetCallInfo(this LambdaExpression expression, Mock mock)
		{
			Guard.NotNull(expression, nameof(expression));

			if (mock.IsDelegateMock)
			{
				// We're a mock for a delegate, so this call can only
				// possibly be the result of invoking the delegate.
				// But the expression we have is for a call on the delegate, not our
				// delegate interface proxy, so we need to map instead to the
				// method on that interface, which is the property we've just tested for.
				var invocation = (InvocationExpression)expression.Body;
				return (Object: invocation.Expression, Method: mock.DelegateInterfaceMethod, Arguments: invocation.Arguments);
			}

			if (expression.Body is MethodCallExpression methodCall)
			{
				return (Object: methodCall.Object, Method: methodCall.Method, Arguments: methodCall.Arguments);
			}

			throw new ArgumentException(string.Format(
				CultureInfo.CurrentCulture,
				Resources.SetupNotMethod,
				expression.ToStringFixed()));
		}
	}
}
