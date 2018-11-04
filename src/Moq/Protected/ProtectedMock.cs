// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Language;
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

		public IProtectedAsMock<T, TAnalog> As<TAnalog>()
			where TAnalog : class
		{
			return new ProtectedAsMock<T, TAnalog>(this.mock);
		}

		#region Setup

		public ISetup<T> Setup(string methodName, params object[] args)
		{
			Guard.NotNullOrEmpty(methodName, nameof(methodName));

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			var setup = Mock.SetupVoid(mock, GetMethodCall(method, args), null);
			return new VoidSetupPhrase<T>(setup);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, params object[] args)
		{
			Guard.NotNullOrEmpty(methodName, nameof(methodName));

			return Setup<TResult>(methodName, false, args);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNullOrEmpty(methodName, nameof(methodName));

			var property = GetProperty(methodName);
			if (property != null)
			{
				ThrowIfPublicGetter(property, typeof(T).Name);
				// TODO should consider property indexers
				var getterSetup = Mock.SetupGet(mock, GetMemberAccess<TResult>(property), null);
				return new NonVoidSetupPhrase<T, TResult>(getterSetup);
			}

			var method = GetMethod(methodName, exactParameterMatch, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfVoidMethod(method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			var setup = Mock.SetupNonVoid(mock, GetMethodCall<TResult>(method, args), null);
			return new NonVoidSetupPhrase<T, TResult>(setup);
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(string propertyName)
		{
			Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicGetter(property, typeof(T).Name);
			Guard.CanRead(property);

			var setup = Mock.SetupGet(mock, GetMemberAccess<TProperty>(property), null);
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		public ISetupSetter<T, TProperty> SetupSet<TProperty>(string propertyName, object value)
		{
			Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicSetter(property, typeof(T).Name);
			Guard.CanWrite(property);

			var setup = Mock.SetupSet(mock, GetSetterExpression(property, ItExpr.IsAny<TProperty>()), null);
			return new SetterSetupPhrase<T, TProperty>(setup);
		}

		public ISetupSequentialAction SetupSequence(string methodOrPropertyName, params object[] args)
		{
			return this.SetupSequence(methodOrPropertyName, false, args);
		}

		public ISetupSequentialAction SetupSequence(string methodOrPropertyName, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNullOrEmpty(methodOrPropertyName, nameof(methodOrPropertyName));

			var method = GetMethod(methodOrPropertyName, exactParameterMatch, args);
			ThrowIfMemberMissing(methodOrPropertyName, method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			var setup = Mock.SetupSequence(mock, GetMethodCall(method, args));
			return new SetupSequencePhrase(setup);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(string methodOrPropertyName, params object[] args)
		{
			return this.SetupSequence<TResult>(methodOrPropertyName, false, args);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(string methodOrPropertyName, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNullOrEmpty(methodOrPropertyName, nameof(methodOrPropertyName));

			var property = GetProperty(methodOrPropertyName);
			if (property != null)
			{
				ThrowIfPublicGetter(property, typeof(T).Name);
				// TODO should consider property indexers
				var getterSetup = Mock.SetupSequence(mock, GetMemberAccess<TResult>(property));
				return new SetupSequencePhrase<TResult>(getterSetup);
			}

			var method = GetMethod(methodOrPropertyName, exactParameterMatch, args);
			ThrowIfMemberMissing(methodOrPropertyName, method);
			ThrowIfVoidMethod(method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			var setup = Mock.SetupSequence(mock, GetMethodCall<TResult>(method, args));
			return new SetupSequencePhrase<TResult>(setup);
		}

		#endregion

		#region Verify

		public void Verify(string methodName, Times times, object[] args)
		{
			Guard.NotNullOrEmpty(methodName, nameof(methodName));

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			Mock.VerifyVoid(mock, GetMethodCall(method, args), times, null);
		}

		public void Verify<TResult>(string methodName, Times times, object[] args)
		{
			Guard.NotNullOrEmpty(methodName, nameof(methodName));

			var property = GetProperty(methodName);
			if (property != null)
			{
				ThrowIfPublicGetter(property, typeof(T).Name);
				// TODO should consider property indexers
				Mock.VerifyGet(mock, GetMemberAccess<TResult>(property), times, null);
				return;
			}

			var method = GetMethod(methodName, args);
			ThrowIfMemberMissing(methodName, method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			Mock.VerifyNonVoid(mock, GetMethodCall<TResult>(method, args), times, null);
		}

		// TODO should receive args to support indexers
		public void VerifyGet<TProperty>(string propertyName, Times times)
		{
			Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicGetter(property, typeof(T).Name);
			Guard.CanRead(property);

			// TODO should consider property indexers
			Mock.VerifyGet(mock, GetMemberAccess<TProperty>(property), times, null);
		}

		// TODO should receive args to support indexers
		public void VerifySet<TProperty>(string propertyName, Times times, object value)
		{
			Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfPublicSetter(property, typeof(T).Name);
			Guard.CanWrite(property);

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
			return GetMethod(methodName, false, args);
		}

		private static MethodInfo GetMethod(string methodName, bool exact, params object[] args)
		{
			var argTypes = ToArgTypes(args);
			return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.SingleOrDefault(m => m.Name == methodName && m.GetParameterTypes().CompareTo(argTypes, exact));
		}

		private static Expression<Func<T, TResult>> GetMethodCall<TResult>(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Func<T, TResult>>(Expression.Call(param, method, ToExpressionArgs(method, args)), param);
		}

		private static Expression<Action<T>> GetMethodCall(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Action<T>>(Expression.Call(param, method, ToExpressionArgs(method, args)), param);
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
				param).CompileUsingExpressionCompiler();
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

		private static void ThrowIfPublicMethod(MethodInfo method, string reflectedTypeName)
		{
			if (method.IsPublic)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MethodIsPublic,
					reflectedTypeName,
					method.Name));
			}
		}

		private static void ThrowIfPublicGetter(PropertyInfo property, string reflectedTypeName)
		{
			if (property.CanRead && property.GetGetMethod() != null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnexpectedPublicProperty,
					reflectedTypeName,
					property.Name));
			}
		}

		private static void ThrowIfPublicSetter(PropertyInfo property, string reflectedTypeName)
		{
			if (property.CanWrite && property.GetSetMethod() != null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnexpectedPublicProperty,
					reflectedTypeName,
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
				throw new ArgumentException(Resources.UseItExprIsNullRatherThanNullArgumentValue);
			}

			var types = new Type[args.Length];
			for (int index = 0; index < args.Length; index++)
			{
				if (args[index] == null)
				{
					throw new ArgumentException(Resources.UseItExprIsNullRatherThanNullArgumentValue);
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
					if (member.Member is FieldInfo field)
					{
						// Test for special case: `It.Ref<TValue>.IsAny`
						if (field.Name == nameof(It.Ref<object>.IsAny))
						{
							var fieldDeclaringType = field.DeclaringType;
							if (fieldDeclaringType.GetTypeInfo().IsGenericType)
							{
								var fieldDeclaringTypeDefinition = fieldDeclaringType.GetGenericTypeDefinition();
								if (fieldDeclaringTypeDefinition == typeof(It.Ref<>))
								{
									types[index] = field.FieldType.MakeByRefType();
									continue;
								}
							}
						}

						types[index] = field.FieldType;
					}
					else if ((member.Member as PropertyInfo) != null)
					{
						types[index] = ((PropertyInfo)member.Member).PropertyType;
					}
					else
					{
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

		private static Expression ToExpressionArg(ParameterInfo paramInfo, object arg)
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

			return Expression.Constant(arg, paramInfo.ParameterType);
		}

		private static IEnumerable<Expression> ToExpressionArgs(MethodInfo method, object[] args)
		{
			ParameterInfo[] methodParams = method.GetParameters();
			for (int i = 0; i < args.Length; i++)
			{
				yield return ToExpressionArg(methodParams[i], args[i]);
			}
		}
	}
}
