// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Expressions.Visitors
{
	/// <summary>
	///   Replaces <see cref="ExpressionType.Call"/> nodes for property or indexer accessor methods
	///   with equivalent <see cref="ExpressionType.MemberAccess"/> nodes.
	///   <para>
	///     <list type="bullet">
	///       <item>
	///         In the case of getter accessors such as `x.get_Property()`, the result will be
	///         a single <see cref="ExpressionType.MemberAccess"/> node: `x.Property`.
	///       </item>
	///       <item>
	///         In the case of setter accessors such as `x.set_Property(y)`, the result will be
	///         a combination of <see cref="ExpressionType.Assign"/> and <see cref="ExpressionType.MemberAccess"/>:
	///         `x.Property = y`.
	///       </item>
	///     </list>
	///   </para>
	/// </summary>
	internal sealed class UpgradePropertyAccessorMethods : ExpressionVisitor
	{
		public static readonly ExpressionVisitor Rewriter = new UpgradePropertyAccessorMethods();

		private UpgradePropertyAccessorMethods()
		{
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var instance = node.Object != null ? this.Visit(node.Object) : null;
			var arguments = this.Visit(node.Arguments);

			if (node.Method.IsSpecialName)
			{
				if (node.Method.IsGetAccessor())
				{
					var name = node.Method.Name.Substring(4);
					var argumentCount = node.Arguments.Count;

					if (argumentCount == 0)
					{
						// getter:
						var property = node.Method.DeclaringType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						Debug.Assert(property != null && property.GetGetMethod(true) == node.Method);

						return Expression.MakeMemberAccess(instance, property);
					}
					else
					{
						// indexer getter:
						var parameterTypes = node.Method.GetParameterTypes();
						var argumentTypes = parameterTypes.ToArray();
						var indexer = node.Method.DeclaringType.GetProperty(name, node.Method.ReturnType, argumentTypes);
						Debug.Assert(indexer != null && indexer.GetGetMethod(true) == node.Method);

						return Expression.MakeIndex(instance, indexer, arguments);
					}
				}
				else if (node.Method.IsSetAccessor())
				{
					var name = node.Method.Name.Substring(4);
					var argumentCount = node.Arguments.Count;

					if (argumentCount == 1)
					{
						// setter:
						var property = node.Method.DeclaringType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						Debug.Assert(property != null && property.GetSetMethod(true) == node.Method);

						var value = node.Arguments[0];
						return Expression.Assign(Expression.MakeMemberAccess(instance, property), value);
					}
					else
					{
						// indexer setter:
						var parameterTypes = node.Method.GetParameterTypes();
						var argumentTypes = parameterTypes.Take(parameterTypes.Count - 1).ToArray();
						var indexer = node.Method.DeclaringType.GetProperty(name, parameterTypes.Last(), argumentTypes);
						Debug.Assert(indexer != null && indexer.GetSetMethod(true) == node.Method);

						var indices = arguments.Take(argumentCount - 1);
						var value = arguments.Last();
						return Expression.Assign(Expression.MakeIndex(instance, indexer, indices), value);
					}
				}
			}

			return instance != node.Object || arguments != node.Arguments ? Expression.Call(instance, node.Method, arguments)
			                                                              : node;
		}
	}
}
