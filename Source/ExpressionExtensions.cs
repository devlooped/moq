//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Moq.Properties;
using IQToolkit;

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
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupNotMethod,
					expression.ToStringFixed()));
			}

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
			return Evaluator.PartialEval(
				expression,
				e => e.NodeType != ExpressionType.Parameter &&
					(e.NodeType != ExpressionType.Call || !ReturnsMatch((MethodCallExpression)e)));
		}

		private static bool ReturnsMatch(MethodCallExpression expression)
		{
			if (expression.Method.GetCustomAttribute<AdvancedMatcherAttribute>(true) == null)
			{
				using (var context = new FluentMockContext())
				{
					Expression.Lambda<Action>(expression).Compile().Invoke();
					return context.LastMatch != null;
				}
			}

			return true;
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
			return new ToStringFixVisitor(expression, false).ExpressionString;
		}

		internal static string ToStringFixed(this Expression expression, bool useFullTypeName)
		{
			return new ToStringFixVisitor(expression, useFullTypeName).ExpressionString;
		}

		internal sealed class RemoveMatcherConvertVisitor : ExpressionVisitor
		{
			public RemoveMatcherConvertVisitor(Expression expression)
			{
				this.Expression = this.Visit(expression);
			}

			protected override Expression VisitUnary(UnaryExpression u)
			{
				if (u.NodeType == ExpressionType.Convert &&
					u.Operand.NodeType == ExpressionType.Call &&
					typeof(Match).IsAssignableFrom(((MethodCallExpression)u.Operand).Method.ReturnType))
				{
					// Remove the cast.
					return u.Operand;
				}

				return base.VisitUnary(u);
			}

			public Expression Expression { get; private set; }
		}

		// TODO: this whole class' approach is weak. We 
		// basically collect all calls in a tree, 
		// which is a graph, but we collect them on a list.
		// Then we do sequential matching of the wrong ToString 
		// and replace with our rendering. Not sure if this 
		// will work for nested calls, etc. Seems to 
		// work fine for the typical mock setups and verification 
		// expressions, but I don't think it's reliable as a 
		// general purpose solution.
		internal sealed class ToStringFixVisitor : ExpressionVisitor
		{
			private List<MethodCallExpression> calls = new List<MethodCallExpression>();
			private Regex convertRegex = new Regex(@"Convert\((.+?)\)");

			public ToStringFixVisitor(Expression expression, bool useFullTypeName)
			{
				this.Visit(expression);

				var fullString = expression.ToString();

				foreach (var call in calls)
				{
					var properCallString = BuildCallExpressionString(call, useFullTypeName);
					var improperCallString = call.ToString();
					// We do it this way so that we replace one call 
					// at a time, as there may be many that match the 
					// improper rendering (i.e. multiple It.IsAny)
					var index = fullString.IndexOf(improperCallString, StringComparison.Ordinal);
					if (index != -1)
					{
						fullString = fullString.Substring(0, index) + properCallString +
									 fullString.Substring(index + improperCallString.Length);
					}
				}

				fullString = convertRegex.Replace(fullString, m => m.Groups[1].Value);

				this.ExpressionString = fullString;
			}

			public string ExpressionString { get; private set; }

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				// We fix generic methods but also renderings of property accesses.
				calls.Add(m);
				return base.VisitMethodCall(m);
			}

			private static string BuildCallExpressionString(MethodCallExpression call, bool useFullTypeName)
			{
				var builder = new StringBuilder();
				var startIndex = 0;
				var targetExpr = call.Object;

				if (Attribute.GetCustomAttribute(call.Method, typeof(ExtensionAttribute)) != null)
				{
					// We should start rendering the args for the invocation from the 
					// second argument.
					startIndex = 1;
					targetExpr = call.Arguments[0];
				}

				if (targetExpr != null)
				{
					builder.Append(targetExpr.ToStringFixed(useFullTypeName));
				}
				else
				{
					// Method is static
					builder.Append(call.Method.DeclaringType.Name);
				}

				if (call.Method.IsSpecialName &&
					call.Method.Name.StartsWith("get_Item", StringComparison.Ordinal))
				{
					builder.Append("[")
						.Append(string.Join(", ", call.Arguments.Select(arg => arg.ToString()).ToArray()))
						.Append("]");
				}
				else if (call.Method.IsSpecialName &&
					call.Method.Name.StartsWith("set_Item", StringComparison.Ordinal))
				{
					builder.Append("[")
						.Append(string.Join(", ", call.Arguments.Take(call.Arguments.Count - 1)
							.Select(arg => arg.ToString()).ToArray()))
						.Append("] = ")
						.Append(call.Arguments.Last().ToStringFixed(useFullTypeName));
				}
				else if (call.Method.IsPropertyGetter())
				{
					builder.Append(".").Append(call.Method.Name.Substring(4));
				}
				else if (call.Method.IsPropertySetter())
				{
					builder.Append(".")
						.Append(call.Method.Name.Substring(4))
						.Append(" = ")
						.Append(call.Arguments.Last().ToStringFixed(useFullTypeName));
				}
				else
				{
					builder.Append(".")
						.Append(useFullTypeName ? call.Method.GetFullName() : call.Method.GetName())
						.Append("(")
						.Append(string.Join(
							", ",
							call.Arguments.Select(a => a.ToString()).ToArray(),
							startIndex,
							call.Arguments.Count - startIndex))
						.Append(")");
				}

				return builder.ToString();
			}
		}
	}
}