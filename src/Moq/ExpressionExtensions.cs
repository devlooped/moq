// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Moq.Async;
using Moq.Properties;
using Moq.Protected;

namespace Moq
{
	internal static partial class ExpressionExtensions
	{
		/// <summary>
		///   Wraps this <paramref name="expression"/> in a <see cref="ExpressionType.Convert"/> node if needed.
		/// </summary>
		/// <param name="expression">The <see cref="Expression"/> which should be wrapped.</param>
		/// <param name="type">The <see cref="Type"/> with which to make the <paramref name="expression"/> compatible.</param>
		/// <remarks>
		///   LINQ expression trees generally enforce type compatibility rules that are stricter than
		///   the assignment-compatibility used by e.g. <see cref="Type.IsAssignableFrom(Type)"/>. For
		///   example, while <see langword="int"/> is assignable-to <see langword="object"/>, you
		///   will need a conversion in a LINQ expression tree to model the value-type boxing operation.
		/// </remarks>
		internal static Expression ConvertIfNeeded(this Expression expression, Type type)
		{
			if (expression.Type == type)
			{
				// if source and target types match exactly,
				// no conversion is necessary:
				return expression;
			}
			if (!expression.Type.IsValueType && !type.IsValueType && type.IsAssignableFrom(expression.Type))
			{
				// if source and target types are reference types and assignment-compatible,
				// no conversion is necessary:
				return expression;
			}
			else
			{
				// everything else requires a conversion:
				return Expression.Convert(expression, type);
			}
		}

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

		public static bool IsMatch(this Expression expression, out Match match)
		{
			if (expression is MatchExpression matchExpression)
			{
				match = matchExpression.Match;
				return true;
			}

			using (var observer = MatcherObserver.Activate())
			{
				Expression.Lambda<Action>(expression).CompileUsingExpressionCompiler().Invoke();
				return observer.TryGetLastMatch(out match);
			}
		}

		/// <summary>
		///   Checks whether the given expression <paramref name="e"/> can be split by <see cref="Split"/>.
		/// </summary>
		public static bool CanSplit(this Expression e)
		{
			switch (e.NodeType)
			{
				case ExpressionType.Assign:
				case ExpressionType.AddAssign:
				case ExpressionType.SubtractAssign:
				{
					var assignmentExpression = (BinaryExpression)e;
					return CanSplit(assignmentExpression.Left);
				}

				case ExpressionType.Call:
				case ExpressionType.Index:
				{
					return true;
				}

				case ExpressionType.Invoke:
				{
					var invocationExpression = (InvocationExpression)e;
					return typeof(Delegate).IsAssignableFrom(invocationExpression.Expression.Type);
				}

				case ExpressionType.MemberAccess:
				{
					var memberAccessExpression = (MemberExpression)e;
					return memberAccessExpression.Member is PropertyInfo;
				}

				case ExpressionType.Parameter:
				default:
				{
					return false;
				}
			}
		}

		/// <summary>
		///   Splits an expression such as `<c>m => m.A.B(x).C[y] = z</c>` into a chain of parts
		///   that can be set up one at a time:
		///   <list>
		///     <item>`<c>m => m.A</c>`</item>,
		///     <item>`<c>... => ....B(x)</c>`</item>,
		///     <item>`<c>... => ....C</c>`</item>,
		///     <item>`<c>... => ...[y] = z</c>`</item>.
		///   </list>
		///   <para>
		///     The split points are chosen such that each part has exactly one associated
		///     <see cref="MethodInfo"/> and optionally some argument expressions.
		///   </para>
		/// </summary>
		/// <exception cref="ArgumentException">
		///   It was not possible to completely split up the expression.
		/// </exception>
		internal static Stack<MethodExpectation> Split(this LambdaExpression expression, bool allowNonOverridableLastProperty = false)
		{
			Debug.Assert(expression != null);

			var parts = new Stack<MethodExpectation>();

			Expression remainder = expression.Body;
			while (CanSplit(remainder))
			{
				Split(remainder, out remainder, out var part, allowNonOverridableLastProperty: allowNonOverridableLastProperty && parts.Count == 0);
				parts.Push(part);
			}

			if (parts.Count > 0 && remainder is ParameterExpression)
			{
				return parts;
			}
			else
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.UnsupportedExpression,
						remainder.ToStringFixed()));
			}

			void Split(Expression e, out Expression r /* remainder */, out MethodExpectation p /* part */, bool assignment = false, bool allowNonOverridableLastProperty = false)
			{
				const string ParameterName = "...";

				switch (e.NodeType)
				{
					case ExpressionType.Assign:          // assignment to a property or indexer
					case ExpressionType.AddAssign:       // subscription of event handler to event
					case ExpressionType.SubtractAssign:  // unsubscription of event handler from event
					{
						var assignmentExpression = (BinaryExpression)e;
						Split(assignmentExpression.Left, out r, out var lhs, assignment: true);
						var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
						var arguments = new Expression[lhs.Method.GetParameters().Length];
						for (var ai = 0; ai < arguments.Length - 1; ++ai)
						{
							arguments[ai] = lhs.Arguments[ai];
						}
						arguments[arguments.Length - 1] = assignmentExpression.Right;
						p = new MethodExpectation(
							expression: Expression.Lambda(
								Expression.MakeBinary(e.NodeType, lhs.Expression.Body, assignmentExpression.Right),
								parameter),
							method: lhs.Method,
							arguments);
						return;
					}

					case ExpressionType.Call:  // regular method call
					{
						var methodCallExpression = (MethodCallExpression)e;

						if (methodCallExpression.Method.IsGenericMethod)
						{
							foreach (var typeArgument in methodCallExpression.Method.GetGenericArguments())
							{
								if (typeArgument.IsOrContainsTypeMatcher())
								{
									// This is a (somewhat roundabout) way of ensuring that the type matchers used
									// will be usable. They will not be usable if they don't implement the type
									// matcher protocol correctly; and `SubstituteTypeMatchers` tests for that, so
									// we'll reuse its recursive logic instead of having to reimplement our own.
									_ = typeArgument.SubstituteTypeMatchers(typeArgument);
								}
							}
						}

						if (!methodCallExpression.Method.IsStatic)
						{
							r = methodCallExpression.Object;
							var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
							var method = methodCallExpression.Method;
							var arguments = methodCallExpression.Arguments;
							p = new MethodExpectation(
										expression: Expression.Lambda(
											Expression.Call(parameter, method, arguments),
											parameter),
										method,
										arguments);
						}
						else
						{
							Debug.Assert(methodCallExpression.Method.IsExtensionMethod());
							Debug.Assert(methodCallExpression.Arguments.Count > 0);
							r = methodCallExpression.Arguments[0];
							var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
							var method = methodCallExpression.Method;
							var arguments = methodCallExpression.Arguments.ToArray();
							arguments[0] = parameter;
							p = new MethodExpectation(
										expression: Expression.Lambda(
											Expression.Call(method, arguments),
											parameter),
										method,
										arguments);
						}
						return;
					}

					case ExpressionType.Index:  // indexer query
					{
						var indexExpression = (IndexExpression)e;
						r = indexExpression.Object;
						var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
						var indexer = indexExpression.Indexer;
						var arguments = indexExpression.Arguments;
						MethodInfo method;
						if (!assignment && indexer.CanRead(out var getter, out var getterIndexer))
						{
							method = getter;
							indexer = getterIndexer;
						}
						else if (indexer.CanWrite(out var setter, out var setterIndexer))
						{
							method = setter;
							indexer = setterIndexer;
						}
						else  // This should be unreachable.
						{
							method = null;
						}
						p = new MethodExpectation(
									expression: Expression.Lambda(
										Expression.MakeIndex(parameter, indexer, arguments),
										parameter),
									method,
									arguments,
									skipMatcherInitialization: assignment,
									allowNonOverridable: allowNonOverridableLastProperty);
						return;
					}

					case ExpressionType.Invoke:  // delegate invocation
					{
						var invocationExpression = (InvocationExpression)e;
						Debug.Assert(invocationExpression.Expression.Type.IsDelegateType());
						r = invocationExpression.Expression;
						var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
						var arguments = invocationExpression.Arguments;
						p = new MethodExpectation(
									expression: Expression.Lambda(
										Expression.Invoke(parameter, arguments),
										parameter),
									method: r.Type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
									arguments);
						return;
					}

					case ExpressionType.MemberAccess:  // property query
					{
						var memberAccessExpression = (MemberExpression)e;
						Debug.Assert(memberAccessExpression.Member is PropertyInfo);

						if (IsResult(memberAccessExpression.Member, out var awaitableFactory))
						{
							Split(memberAccessExpression.Expression, out r, out p);
							p.AddResultExpression(
								awaitable => Expression.MakeMemberAccess(awaitable, memberAccessExpression.Member),
								awaitableFactory);
							return;
						}

						r = memberAccessExpression.Expression;
						var parameter = Expression.Parameter(r.Type, r is ParameterExpression ope ? ope.Name : ParameterName);
						var property = memberAccessExpression.GetReboundProperty();
						MethodInfo method;
						if (!assignment && property.CanRead(out var getter, out var getterProperty))
						{
							method = getter;
							property = getterProperty;
						}
						else if (property.CanWrite(out var setter, out var setterProperty))
						{
							method = setter;
							property = setterProperty;
						}
						else  // This should be unreachable.
						{
							method = null;
						}
						p = new MethodExpectation(
									expression: Expression.Lambda(
										Expression.MakeMemberAccess(parameter, property),
										parameter),
									method,
									skipMatcherInitialization: assignment,
									allowNonOverridable: allowNonOverridableLastProperty);
						return;
					}

					default:
						Debug.Assert(!CanSplit(e));
						throw new InvalidOperationException();  // this should be unreachable
				}
			}

			bool IsResult(MemberInfo member, out IAwaitableFactory awaitableFactory)
			{
				var instanceType = member.DeclaringType;
				awaitableFactory = AwaitableFactory.TryGet(instanceType);
				var returnType = member switch { PropertyInfo p => p.PropertyType,
				                                 _              => null };
				return awaitableFactory != null && object.Equals(returnType, awaitableFactory.ResultType);
			}
		}

		internal static PropertyInfo GetReboundProperty(this MemberExpression expression)
		{
			Debug.Assert(expression.Member is PropertyInfo);

			var property = (PropertyInfo)expression.Member;

			// the following block is required because .NET compilers put the wrong PropertyInfo into MemberExpression
			// for properties originally declared in base classes; they will put the base class' PropertyInfo into
			// the expression. we attempt to correct this here by checking whether the type of the accessed object
			// has a property by the same name whose base definition equals the property in the expression; if so,
			// we "upgrade" to the derived property.
			if (property.DeclaringType != expression.Expression.Type)
			{
				var parameterTypes = new ParameterTypes(property.GetIndexParameters());
				var derivedProperty = expression.Expression.Type
					.GetMember(property.Name, MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.Cast<PropertyInfo>()
					.SingleOrDefault(p =>
					{
						return p.PropertyType == property.PropertyType
							&& new ParameterTypes(p.GetIndexParameters()).CompareTo(parameterTypes, true, false);
					});
				if (derivedProperty != null)
				{
					if ((derivedProperty.CanRead(out var getter) && getter.GetBaseDefinition() == property.GetGetMethod(true)) ||
						(derivedProperty.CanWrite(out var setter) && setter.GetBaseDefinition() == property.GetSetMethod(true)))
					return derivedProperty;
				}
			}

			return property;
		}

		/// <summary>
		/// Converts the body of the lambda expression into the <see cref="PropertyInfo"/> referenced by it.
		/// </summary>
		public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
		{
			if (expression.Body is MemberExpression prop)
			{
				return prop.GetReboundProperty();
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
			Debug.Assert(expression != null);

			return expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo;
		}

		/// <summary>
		///   Checks whether the body of the lambda expression is a indexer access.
		/// </summary>
		public static bool IsPropertyIndexer(this LambdaExpression expression)
		{
			Debug.Assert(expression != null);

			return expression.Body is IndexExpression
				|| (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.IsSpecialName);
		}

		public static Expression<Action<TMock>> AssignItIsAny<TMock, T>(this Expression<Func<TMock, T>> expression)
		{
			Debug.Assert(expression != null);
			Debug.Assert(expression.Body is MemberExpression || expression.Body is IndexExpression);

			return Expression.Lambda<Action<TMock>>(
					Expression.Assign(
							expression.Body,
							ItExpr.IsAny<T>()),
					expression.Parameters[0]);
		}

		public static Expression PartialEval(this Expression expression)
		{
			return Evaluator.PartialEval(expression);
		}

		public static Expression PartialMatcherAwareEval(this Expression expression)
		{
			return Evaluator.PartialEval(
				expression,
				PartialMatcherAwareEval_ShouldEvaluate);
		}

		private static bool PartialMatcherAwareEval_ShouldEvaluate(Expression expression)
		{
#pragma warning disable 618
			return expression.NodeType switch
			{
				ExpressionType.Quote        => false,
				ExpressionType.Parameter    => false,
				ExpressionType.Extension    => !(expression is MatchExpression),
				ExpressionType.Call         => !((MethodCallExpression)expression).Method.IsDefined(typeof(MatcherAttribute), true)
				                               && !expression.IsMatch(out _),
				ExpressionType.MemberAccess => !expression.IsMatch(out _),
				_                           => true,
			};
#pragma warning restore 618
		}

		/// <devdoc>
		/// TODO: remove this code when https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=331583 
		/// is fixed.
		/// </devdoc>
		public static string ToStringFixed(this Expression expression)
		{
			return new StringBuilder().AppendExpression(expression).ToString();
		}
	}
}
