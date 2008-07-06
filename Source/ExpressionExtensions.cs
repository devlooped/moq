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
using System.Runtime.CompilerServices;
using System.Text;

namespace Moq
{
	internal static class ExpressionExtensions
	{
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

		internal class ToStringFixVisitor : ExpressionVisitor
		{
			string expressionString;
			List<MethodCallExpression> calls = new List<MethodCallExpression>();

			public ToStringFixVisitor(Expression expression)
			{
				Visit(expression);

				string fullString = expression.ToString();

				foreach (var call in calls)
				{
					string properCallString = BuildCallExpressionString(call);

					fullString = fullString.Replace(call.ToString(), properCallString);
				}

				expressionString = fullString;
			}

			private string BuildCallExpressionString(MethodCallExpression call)
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

			public string ExpressionString { get { return expressionString; } }

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				// We only need to fix generic methods.
				if (m.Method.IsGenericMethod)
					calls.Add(m);

				return base.VisitMethodCall(m);
			}
		}
	}
}
