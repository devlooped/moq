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
using Moq.Language.Flow;
using Moq.Properties;

namespace Moq.Protected
{
	internal class ProtectedMock<T> : IProtectedMock<T>
			where T : class
	{
		private Mock<T> mock;

		public ProtectedMock(Mock<T> mock)
		{
			this.mock = mock;
		}

		#region Setup

		public ISetup<T> Setup(string methodName, params object[] args)
		{
			Guard.NotNullOrEmpty(() => methodName, methodName);

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method);

			return Mock.Setup(mock, GetMethodCall(method, args), null);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, params object[] args)
		{
			Guard.NotNullOrEmpty(() => methodName, methodName);

			var property = GetProperty(methodName);
			if (property != null)
			{
				ThrowIfPublicGetter(property);
				// TODO should consider property indexers
				return Mock.SetupGet(mock, GetMemberAccess<TResult>(property), null);
			}

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfVoidMethod(method);
			ThrowIfPublicMethod(method);

			return Mock.Setup(mock, GetMethodCall<TResult>(method, args), null);
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(string propertyName)
		{
			Guard.NotNullOrEmpty(() => propertyName, propertyName);

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicGetter(property);
			property.ThrowIfNoGetter();

			return Mock.SetupGet(mock, GetMemberAccess<TProperty>(property), null);
		}

		public ISetupSetter<T, TProperty> SetupSet<TProperty>(string propertyName, object value)
		{
			Guard.NotNullOrEmpty(() => propertyName, propertyName);

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicSetter(property);
			property.ThrowIfNoSetter();

			return Mock.SetupSet<T, TProperty>(mock, GetSetterExpression(property, ItExpr.IsAny<TProperty>()), null);
		}

		#endregion

		#region Verify

		public void Verify(string methodName, Times times, object[] args)
		{
			Guard.NotNullOrEmpty(() => methodName, methodName);

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method);

			Mock.Verify(mock, GetMethodCall(method, args), times, null);
		}

		public void Verify<TResult>(string methodName, Times times, object[] args)
		{
			Guard.NotNullOrEmpty(() => methodName, methodName);

			var property = GetProperty(methodName);
			if (property != null)
			{
				ThrowIfPublicGetter(property);
				// TODO should consider property indexers
				Mock.VerifyGet(mock, GetMemberAccess<TResult>(property), times, null);
				return;
			}

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method);

			Mock.Verify(mock, GetMethodCall<TResult>(method, args), times, null);
		}

		// TODO should receive args to support indexers
		public void VerifyGet<TProperty>(string propertyName, Times times)
		{
			Guard.NotNullOrEmpty(() => propertyName, propertyName);

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicGetter(property);
			property.ThrowIfNoGetter();

			// TODO should consider property indexers
			Mock.VerifyGet(mock, GetMemberAccess<TProperty>(property), times, null);
		}

		// TODO should receive args to support indexers
		public void VerifySet<TProperty>(string propertyName, Times times, object value)
		{
			Guard.NotNullOrEmpty(() => propertyName, propertyName);

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicSetter(property);
			property.ThrowIfNoSetter();

			// TODO should consider property indexers
			// TODO should receive the parameter here
			Mock.VerifySet(mock, GetSetterExpression(property, ItExpr.IsAny<TProperty>()), times, null);
		}

		#endregion

		private static Expression<Func<T, TResult>> GetMemberAccess<TResult>(PropertyInfo property)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Func<T, TResult>>(Expression.MakeMemberAccess(param, property), param);
		}

		private static MethodInfo GetMethod(string methodName, params object[] args)
		{
			return typeof(T).GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				null,
				ToArgTypes(args),
				null);
		}

		private static Expression<Func<T, TResult>> GetMethodCall<TResult>(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Func<T, TResult>>(Expression.Call(param, method, ToExpressionArgs(args)), param);
		}

		private static Expression<Action<T>> GetMethodCall(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Action<T>>(Expression.Call(param, method, ToExpressionArgs(args)), param);
		}

		// TODO should support arguments for property indexers
		private static PropertyInfo GetProperty(string propertyName)
		{
			return typeof(T).GetProperty(
				propertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}

		private static Action<T> GetSetterExpression(PropertyInfo property, Expression value)
		{
			var param = Expression.Parameter(typeof(T), "mock");

			return Expression.Lambda<Action<T>>(
				Expression.Call(param, property.GetSetMethod(true), value),
				param).Compile();
		}

		private static void ThrowIfNonVirtual(MethodInfo method)
		{
			if (method.IsAssembly || method.IsFamilyAndAssembly)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.VerifyOnNonVirtualMember,
					method.ReflectedType.Name + "." + method.Name));
			}
		}

		private static void ThrowIfMemberMissing(string memberName, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MemberMissing,
					typeof(T).Name,
					memberName));
			}
		}

		private static void ThrowIfPublicMethod(MethodInfo method)
		{
			if (method.IsPublic)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MethodIsPublic,
					method.ReflectedType.Name,
					method.Name));
			}
		}

		private static void ThrowIfPublicGetter(PropertyInfo property)
		{
			if (property.CanRead && property.GetGetMethod() != null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnexpectedPublicProperty,
					property.ReflectedType.Name,
					property.Name));
			}
		}

		private static void ThrowIfPublicSetter(PropertyInfo property)
		{
			if (property.CanWrite && property.GetSetMethod() != null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnexpectedPublicProperty,
					property.ReflectedType.Name,
					property.Name));
			}
		}

		private static void ThrowIfVoidMethod(MethodInfo method)
		{
			if (method.ReturnType == typeof(void))
			{
				throw new ArgumentException(Resources.CantSetReturnValueForVoid);
			}
		}

		private static Type[] ToArgTypes(object[] args)
		{
			if (args == null)
			{
				throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
			}

			var types = new Type[args.Length];
			for (int index = 0; index < args.Length; index++)
			{
				if (args[index] == null)
				{
					throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
				}

				var expr = args[index] as Expression;
				if (expr == null)
				{
					types[index] = args[index].GetType();
				}
				else if (expr.NodeType == ExpressionType.Call)
				{
					types[index] = ((MethodCallExpression)expr).Method.ReturnType;
				}
				else if (expr.NodeType == ExpressionType.MemberAccess)
				{
					var member = (MemberExpression)expr;
					switch (member.Member.MemberType)
					{
						case MemberTypes.Field:
							types[index] = ((FieldInfo)member.Member).FieldType;
							break;
						case MemberTypes.Property:
							types[index] = ((PropertyInfo)member.Member).PropertyType;
							break;
						default:
							throw new NotSupportedException(string.Format(
								Resources.Culture,
								Resources.UnsupportedMember,
								member.Member.Name));
					}
				}
				else
				{
					var evalExpr = expr.PartialEval();
					if (evalExpr.NodeType == ExpressionType.Constant)
					{
						types[index] = ((ConstantExpression)evalExpr).Type;
					}
					else
					{
						types[index] = null;
					}
				}
			}

			return types;
		}

		private static Expression ToExpressionArg(object arg)
		{
			var lambda = arg as LambdaExpression;
			if (lambda != null)
			{
				return lambda.Body;
			}

			var expression = arg as Expression;
			if (expression != null)
			{
				return expression;
			}

			return Expression.Constant(arg);
		}

		private static IEnumerable<Expression> ToExpressionArgs(object[] args)
		{
			return args.Select(arg => ToExpressionArg(arg));
		}
	}
}