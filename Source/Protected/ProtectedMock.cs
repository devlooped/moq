using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;
using System.Reflection;
using System.Linq.Expressions;

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

		//public IExpect<TResult> Expect<TResult>(string methodOrProperty, params object[] args)
		//{
		//   Guard.ArgumentNotNull(mock, "mock");

		//   var method = typeof(T).GetMethod(methodOrProperty,
		//      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		//   var property = typeof(T).GetProperty(methodOrProperty,
		//      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		//   ThrowIfMemberMissing(methodOrProperty, method, property);

		//   if (method != null)
		//      return ExpectMethod(method, args);
		//   else
		//      throw new NotSupportedException("Protected properties not supported yet.");
		//   //   return ExpectProperty(mock, property, args);
		//}

		public IExpect Expect(string voidMethodName, params object[] args)
		{
			Guard.ArgumentNotNullOrEmptyString(voidMethodName, "voidMethodName");

			var method = typeof(T).GetMethod(voidMethodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			var property = typeof(T).GetProperty(voidMethodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfMemberMissing(voidMethodName, method, property);

			VerifyMethod(method);
			VerifyProperty(property);

			if (method != null)
			{
				var param = Expression.Parameter(typeof(T), "x");

				return mock.Expect(Expression.Lambda<Action<T>>(
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

		public IExpect<TResult> Expect<TResult>(string methodOrPropertyName, params object[] args)
		{
			Guard.ArgumentNotNullOrEmptyString(methodOrPropertyName, "methodOrPropertyName");

			var method = typeof(T).GetMethod(methodOrPropertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			var property = typeof(T).GetProperty(methodOrPropertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfMemberMissing(methodOrPropertyName, method, property);

			VerifyMethod(method);
			VerifyProperty(property);

			var param = Expression.Parameter(typeof(T), "x");

			if (method != null)
			{
				return mock.Expect(Expression.Lambda<Func<T, TResult>>(
						Expression.Call(param, method, ToExpressionArgs(args)),
						param));
			}
			else
			{
				return mock.Expect(Expression.Lambda<Func<T, TResult>>(
						Expression.MakeMemberAccess(param, property),
						param));
			}
		}

		public IExpectSetter<TProperty> ExpectSet<TProperty>(string propertyName, params object[] args)
		{
			Guard.ArgumentNotNullOrEmptyString(propertyName, "propertyName");

			var property = typeof(T).GetProperty(propertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			ThrowIfPropertyMissing(propertyName, property);
			VerifyProperty(property);

			var param = Expression.Parameter(typeof(T), "x");

			return mock.ExpectSet(Expression.Lambda<Func<T, TProperty>>(
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

				if (method.IsAssembly || method.IsFamilyOrAssembly || method.IsFamilyAndAssembly)
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

		private static IEnumerable<Expression> ToExpressionArgs(object[] args)
		{
			// TODO: take into account lambdas, expressions, etc.
			foreach (var arg in args)
			{
				yield return Expression.Constant(arg);
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
