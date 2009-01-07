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
using Moq.Language.Flow;

namespace Moq.Protected
{
	internal class ProtectedMock<T> : IProtectedMock
			where T : class
	{
		Mock<T> mock;

		public ProtectedMock(Mock<T> mock)
		{
			this.mock = mock;
		}

		public ISetup Expect(string voidMethodName, params object[] args)
		{
			Guard.ArgumentNotNullOrEmptyString(voidMethodName, "voidMethodName");

			var method = typeof(T).GetMethod(voidMethodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				null,
				ToArgTypes(args).ToArray(),
				null);

			var property = typeof(T).GetProperty(voidMethodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfMemberMissing(voidMethodName, method, property);

			VerifyMethod(method);
			VerifyProperty(property);

			if (method != null)
			{
				var param = Expression.Parameter(typeof(T), "x");

				return mock.Setup(Expression.Lambda<Action<T>>(
						Expression.Call(param, method, ToExpressionArgs(args)),
						param));
			}
			else
			{
				throw new ArgumentException(String.Format(
					Properties.Resources.UnsupportedProtectedProperty,
					property.ReflectedType.Name,
					property.Name));
			}
		}

		public ISetup<TResult> Expect<TResult>(string methodOrPropertyName, params object[] args)
		{
			Guard.ArgumentNotNullOrEmptyString(methodOrPropertyName, "methodOrPropertyName");

			var method = typeof(T).GetMethod(methodOrPropertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				null,
				ToArgTypes(args).ToArray(),
				null);

			var property = typeof(T).GetProperty(methodOrPropertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfMemberMissing(methodOrPropertyName, method, property);

			VerifyMethod(method);
			VerifyProperty(property);

			var param = Expression.Parameter(typeof(T), "x");

			if (method != null)
			{
				if (method.ReturnType == typeof(void))
					throw new ArgumentException(Properties.Resources.CantSetReturnValueForVoid);

				return mock.Setup(Expression.Lambda<Func<T, TResult>>(
						Expression.Call(param, method, ToExpressionArgs(args)),
						param));
			}
			else
			{
				return mock.Setup(Expression.Lambda<Func<T, TResult>>(
						Expression.MakeMemberAccess(param, property),
						param));
			}
		}

		public ISetupGetter<TProperty> ExpectGet<TProperty>(string propertyName)
		{
			Guard.ArgumentNotNullOrEmptyString(propertyName, "propertyName");

			var property = typeof(T).GetProperty(propertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfPropertyMissing(propertyName, property);
			VerifyProperty(property);

			var param = Expression.Parameter(typeof(T), "x");

			return mock.SetupGet(Expression.Lambda<Func<T, TProperty>>(
					Expression.MakeMemberAccess(param, property),
					param));
		}

		public ISetupSetter<TProperty> ExpectSet<TProperty>(string propertyName)
		{
			Guard.ArgumentNotNullOrEmptyString(propertyName, "propertyName");

			var property = typeof(T).GetProperty(propertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfPropertyMissing(propertyName, property);
			VerifyProperty(property);

			var param = Expression.Parameter(typeof(T), "x");

			return mock.SetupSet(Expression.Lambda<Func<T, TProperty>>(
					Expression.MakeMemberAccess(param, property),
					param));
		}

		private void VerifyMethod(MethodInfo method)
		{
			if (method != null)
			{
				if (method.IsPublic)
					throw new ArgumentException(String.Format(
						Properties.Resources.MethodIsPublic,
						method.ReflectedType.Name,
						method.Name));

				if (method.IsAssembly || method.IsFamilyAndAssembly)
					throw new ArgumentException(String.Format(
						Properties.Resources.ExpectationOnNonOverridableMember,
						method.ReflectedType.Name + "." + method.Name));
			}
		}

		private void VerifyProperty(PropertyInfo property)
		{
			if (property != null &&
				((property.CanRead && property.GetGetMethod() != null ||
				(property.CanWrite && property.GetSetMethod() != null))))
			{
				throw new ArgumentException(String.Format(
					Properties.Resources.UnexpectedPublicProperty,
					property.ReflectedType.Name,
					property.Name));
			}
		}

		private IEnumerable<Type> ToArgTypes(object[] args)
		{
			if (args != null)
			{
				foreach (var arg in args)
				{
					if (arg == null)
					{
						throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
					}
					else
					{
						var expr = arg as Expression;
						if (expr == null)
						{
							yield return arg.GetType();
						}
						else
						{
							if (expr.NodeType == ExpressionType.Call)
							{
								yield return ((MethodCallExpression)expr).Method.ReturnType;
							}
							else if (expr.NodeType == ExpressionType.MemberAccess)
							{
								var member = (MemberExpression)expr;

								switch (member.Member.MemberType)
								{
									case MemberTypes.Field:
										yield return ((FieldInfo)member.Member).FieldType;
										break;
									case MemberTypes.Property:
										yield return ((PropertyInfo)member.Member).PropertyType;
										break;
									default:
										throw new NotSupportedException(String.Format(
											Properties.Resources.UnsupportedMember,
											member.Member.Name));
								}
							}
							else
							{
								var evalExpr = expr.PartialEval();

								if (evalExpr.NodeType == ExpressionType.Constant)
									yield return ((ConstantExpression)evalExpr).Type;
								else
									yield return null;
							}
						}
					}
				}
			}
			else
			{
				throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
			}
		}

		private static IEnumerable<Expression> ToExpressionArgs(object[] args)
		{
			foreach (var arg in args)
			{
				var expr = arg as Expression;
				if (expr != null)
				{
					if (expr.NodeType == ExpressionType.Lambda)
						yield return ((LambdaExpression)expr).Body;
					else
						yield return expr;
				}
				else
				{
					yield return Expression.Constant(arg);
				}
			}
		}

		private void ThrowIfPropertyMissing(string propertyName, PropertyInfo property)
		{
			if (property == null)
			{
				throw new ArgumentException(String.Format(
					Properties.Resources.MemberMissing,
					typeof(T).Name, propertyName));
			}
		}

		private static void ThrowIfMemberMissing(string memberName, MethodInfo method, PropertyInfo property)
		{
			if (method == null && property == null)
			{
				throw new ArgumentException(String.Format(
					Properties.Resources.MemberMissing,
					typeof(T).Name, memberName));
			}
		}
	}
}
