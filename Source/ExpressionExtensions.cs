//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
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

//    * Neither the name of the Moq Team nor the 
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Globalization;

namespace Moq
{
	internal static class ExpressionExtensions
	{
		/// <summary>
		/// Casts the expression to a lambda expression, removing 
		/// a cast if there's any.
		/// </summary>
		public static LambdaExpression ToLambda(this Expression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			LambdaExpression lambda = expression as LambdaExpression;
			if (lambda == null)
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.UnsupportedExpression, expression));

			// Remove convert expressions which are passed-in by the MockProtectedExtensions.
			// They are passed because LambdaExpression constructor checks the type of 
			// the returned values, even if the return type is Object and everything 
			// is able to convert to it. It forces you to be explicit about the conversion.
			var convert = lambda.Body as UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
				lambda = Expression.Lambda(convert.Operand, lambda.Parameters.ToArray());

			return lambda;
		}

		/// <summary>
		/// Casts the body of the lambda expression to a <see cref="MethodCallExpression"/>.
		/// </summary>
		/// <exception cref="ArgumentException">If the body is not a method call.</exception>
		public static MethodCallExpression ToMethodCall(this LambdaExpression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			var methodCall = expression.Body as MethodCallExpression;

			if (methodCall == null)
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.SetupNotMethod, expression.ToStringFixed()));
			else
				return methodCall;
		}

		/// <summary>
		/// Converts the body of the lambda expression into the <see cref="PropertyInfo"/> referenced by it.
		/// </summary>
		public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
		{
			var prop = expression.Body as MemberExpression;

			if (prop != null)
			{
				var info = prop.Member as PropertyInfo;
				if (info != null)
				{
					return info;
				}
			}

			throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
				Properties.Resources.SetupNotProperty, expression.ToStringFixed()));
		}

		/// <summary>
		/// Checks whether the body of the lambda expression is a property access.
		/// </summary>
		public static bool IsProperty(this LambdaExpression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			return IsProperty(expression.Body);
		}

		/// <summary>
		/// Checks whether the expression is a property access.
		/// </summary>
		public static bool IsProperty(this Expression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			var prop = expression as MemberExpression;

			return prop != null && prop.Member is PropertyInfo;
		}

		/// <summary>
		/// Checks whether the body of the lambda expression is a property indexer, which is true 
		/// when the expression is an <see cref="MethodCallExpression"/> whose 
		/// <see cref="MethodCallExpression.Method"/> has <see cref="MethodBase.IsSpecialName"/> 
		/// equal to <see langword="true"/>.
		/// </summary>
		public static bool IsPropertyIndexer(this LambdaExpression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			return IsPropertyIndexer(expression.Body);
		}

		/// <summary>
		/// Checks whether the expression is a property indexer, which is true 
		/// when the expression is an <see cref="MethodCallExpression"/> whose 
		/// <see cref="MethodCallExpression.Method"/> has <see cref="MethodBase.IsSpecialName"/> 
		/// equal to <see langword="true"/>.
		/// </summary>
		public static bool IsPropertyIndexer(this Expression expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			var call = expression as MethodCallExpression;

			return call != null && call.Method.IsSpecialName;
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

		public static Expression PartialMatcherAwareEval(this Expression expression)
		{
			return Evaluator.PartialEval(expression,
				e => e.NodeType != ExpressionType.Parameter &&
					!(e.NodeType == ExpressionType.Call &&
					((MethodCallExpression)e).Method.GetCustomAttribute<AdvancedMatcherAttribute>(true) != null)
			);
		}

		/// <summary>
		/// Creates an expression that casts the given expression to the <typeparamref name="T"/> 
		/// type.
		/// </summary>
		public static Expression CastTo<T>(this Expression expression)
		{
			return Expression.Convert(expression, typeof(T));
		}

		/// <devdoc>
		/// TODO: remove this code when https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=331583 
		/// is fixed.
		/// </devdoc>
		public static string ToStringFixed(this Expression expression)
		{
			return new ToStringFixVisitor(expression).ExpressionString;
		}

		internal sealed class ToStringFixVisitor : ExpressionVisitor
		{
			private List<MethodCallExpression> calls = new List<MethodCallExpression>();

			public ToStringFixVisitor(Expression expression)
			{
				this.Visit(expression);

				string fullString = expression.ToString();

				foreach (var call in calls)
				{
					string properCallString = BuildCallExpressionString(call);
					string improperCallString = call.ToString();

					int index = fullString.IndexOf(improperCallString);
					if (index != -1)
					{
						fullString = fullString.Substring(0, index) + properCallString +
									 fullString.Substring(index + improperCallString.Length);
					}
				}

				this.ExpressionString = fullString;
			}

			private static string BuildCallExpressionString(MethodCallExpression call)
			{
				var builder = new StringBuilder();
				int startIndex = 0;
				Expression targetExpr = call.Object;

				if (Attribute.GetCustomAttribute(call.Method, typeof(ExtensionAttribute)) != null)
				{
					// We should start rendering the args for the invocation from the 
					// second argument.
					startIndex = 1;
					targetExpr = call.Arguments[0];
				}

				if (targetExpr != null)
				{
					builder.Append(targetExpr.ToString());
					builder.Append(".");
				}

				builder.Append(call.Method.Name);
				if (call.Method.IsGenericMethod)
				{
					builder.Append("<");
					builder.Append(String.Join(", ",
						(from arg in call.Method.GetGenericArguments()
						 select arg.Name).ToArray()));
					builder.Append(">");
				}

				builder.Append("(");

				builder.Append(String.Join(", ",
					(from c in call.Arguments
					 select c.ToString()).ToArray(),
					startIndex, call.Arguments.Count - startIndex));

				builder.Append(")");

				string properCallString = builder.ToString();
				return properCallString;
			}

			public string ExpressionString { get; private set; }

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				// We only need to fix generic methods.
				if (m.Method.IsGenericMethod)
				{
					calls.Add(m);
				}

				return base.VisitMethodCall(m);
			}
		}
	}
}