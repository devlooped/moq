// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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

using TypeNameFormatter;

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
			return new ProtectedAsMock<T, TAnalog>(mock);
		}

		#region Setup

		public ISetup<T> Setup(string methodName, params object[] args)
		{
			return this.InternalSetup(methodName, null, false, args);
		}

		public ISetup<T> Setup(string methodName, bool exactParameterMatch, params object[] args)
		{
			return this.InternalSetup(methodName, null, exactParameterMatch, args);
		}

		public ISetup<T> Setup(string methodName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));

			return this.InternalSetup(methodName, genericTypeArguments, exactParameterMatch, args);
		}

		private ISetup<T> InternalSetup(string methodName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			MethodCall setup = null;
			GetSetterIncludingIndexerOrMethodExpression(methodName, nameof(methodName), genericTypeArguments, exactParameterMatch, args,
				propertyExpression =>
				{
					setup = Mock.SetupSet(mock, propertyExpression, null);
				},
				methodExpression =>
				{
					setup = Mock.Setup(mock, methodExpression, null);
				}
			);

			return new VoidSetupPhrase<T>(setup);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, params object[] args)
		{
			return this.InternalSetup<TResult>(methodName, null, false, args);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, bool exactParameterMatch, params object[] args)
		{
			return this.InternalSetup<TResult>(methodName, null, exactParameterMatch, args);
		}

		public ISetup<T, TResult> Setup<TResult>(string methodName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));

			return this.InternalSetup<TResult>(methodName, genericTypeArguments, exactParameterMatch, args);
		}

		private ISetup<T, TResult> InternalSetup<TResult>(string methodName, Type[] genericTypeArguments,
			bool exactParameterMatch, params object[] args)
		{
			var result = GetGetterIncludingIndexerOrMethodExpression<TResult>(methodName, nameof(methodName), genericTypeArguments, exactParameterMatch, args);
			MethodCall methodCall;
			if (result.Item2)
			{
				methodCall = Mock.SetupGet(mock, result.Item1, null);
			}
			else
			{
				methodCall = Mock.Setup(mock, result.Item1, null);
			}

			return new NonVoidSetupPhrase<T, TResult>(methodCall);
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(string propertyName)
		{
			var setup = Mock.SetupGet(mock, GetGetterExpression<TProperty>(propertyName), null);
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		public ISetup<T, TProperty> SetupGet<TProperty>(string indexerName, object[] indexerKeys)
		{
			var expression = GetIndexerGetterExpression(indexerName, indexerKeys);
			var setup = Mock.SetupGet(mock, expression, null);
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		public ISetupSetter<T, TProperty> SetupSet<TProperty>(string propertyName, object value)
		{
			return new SetterSetupPhrase<T, TProperty>(CommonSetupSet(propertyName, value));
		}

		public ISetup<T> SetupSet(string propertyName, object value)
		{
			return new VoidSetupPhrase<T>(CommonSetupSet(propertyName, value));
		}

		private MethodCall CommonSetupSet(string propertyName, object value)
		{
			var expression = GetSetterExpressionIncludingIndexer(propertyName, value, false, nameof(propertyName));
			return Mock.SetupSet(mock, expression, condition: null);
		}

		public ISetup<T> SetupSet(string indexerName, object[] indexerKeys, object value)
		{
			var expression = GetIndexerSetterExpression(indexerName, indexerKeys, value);
			var methodCall = Mock.SetupSet(mock, expression, condition: null);
			return new VoidSetupPhrase<T>(methodCall);
		}

		public ISetupSequentialAction SetupSequence(string methodOrPropertyName, params object[] args)
		{
			return this.InternalSetupSequence(methodOrPropertyName, null, false, args);
		}

		public ISetupSequentialAction SetupSequence(string methodOrPropertyName, bool exactParameterMatch, params object[] args)
		{
			return this.InternalSetupSequence(methodOrPropertyName, null, exactParameterMatch, args);
		}

		public ISetupSequentialAction SetupSequence(string methodOrPropertyName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			return this.InternalSetupSequence(methodOrPropertyName, genericTypeArguments, exactParameterMatch, args);
		}

		private ISetupSequentialAction InternalSetupSequence(string methodOrPropertyName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			LambdaExpression expression = null;
			GetSetterIncludingIndexerOrMethodExpression(methodOrPropertyName, nameof(methodOrPropertyName), genericTypeArguments, exactParameterMatch, args,
				propertyExpression => expression = propertyExpression,
				methodExpression => expression = methodExpression
			);

			var setup = Mock.SetupSequence(mock, expression);
			return new SetupSequencePhrase(setup);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(string methodOrPropertyName, params object[] args)
		{
			return this.InternalSetupSequence<TResult>(methodOrPropertyName, null, false, args);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(string methodOrPropertyName, bool exactParameterMatch, params object[] args)
		{
			return this.InternalSetupSequence<TResult>(methodOrPropertyName, null, exactParameterMatch, args);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(string methodOrPropertyName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			return this.InternalSetupSequence<TResult>(methodOrPropertyName, genericTypeArguments, exactParameterMatch, args);
		}

		private ISetupSequentialResult<TResult> InternalSetupSequence<TResult>(string methodOrPropertyName, Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			var expression = GetGetterIncludingIndexerOrMethodExpression<TResult>(
				methodOrPropertyName, nameof(methodOrPropertyName), genericTypeArguments, exactParameterMatch, args).Item1;
			

			var setup = Mock.SetupSequence(mock, expression);
			return new SetupSequencePhrase<TResult>(setup);
		}

		#endregion

		#region Verify

		public void Verify(string methodName, Times times, object[] args)
		{
			this.InternalVerify(methodName, null, times, false, args);
		}

		public void Verify(string methodName, Type[] genericTypeArguments, Times times, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			this.InternalVerify(methodName, genericTypeArguments, times, false, args);
		}

		public void Verify(string methodName, Times times, bool exactParameterMatch, object[] args)
		{
			this.InternalVerify(methodName, null, times, exactParameterMatch, args);
		}

		public void Verify(string methodName, Type[] genericTypeArguments, Times times, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			this.InternalVerify(methodName, genericTypeArguments, times, exactParameterMatch, args);
		}

		private void InternalVerify(string methodName, Type[] genericTypeArguments, Times times, bool exactParameterMatch, params object[] args)
		{
			GetSetterIncludingIndexerOrMethodExpression(methodName, nameof(methodName), genericTypeArguments, exactParameterMatch, args,
				propertyExpression =>
				{
					Mock.VerifySet(mock, propertyExpression, times, null);
				},
				methodExpression =>
				{
					Mock.Verify(mock, methodExpression, times, null);
				}
			);
		}

		public void Verify<TResult>(string methodName, Times times, object[] args)
		{
			this.InternalVerify<TResult>(methodName, null, times, false, args);
		}

		public void Verify<TResult>(string methodName, Type[] genericTypeArguments, Times times, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			this.InternalVerify<TResult>(methodName, genericTypeArguments, times, false, args);
		}

		public void Verify<TResult>(string methodName, Times times, bool exactParameterMatch, object[] args)
		{
			this.InternalVerify<TResult>(methodName, null, times, exactParameterMatch, args);
		}

		public void Verify<TResult>(string methodName, Type[] genericTypeArguments, Times times, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNull(genericTypeArguments, nameof(genericTypeArguments));
			this.InternalVerify<TResult>(methodName, null, times, exactParameterMatch, args);
		}

		private void InternalVerify<TResult>(string methodName, Type[] genericTypeArguments, Times times, bool exactParameterMatch, params object[] args)
		{
			var result = GetGetterIncludingIndexerOrMethodExpression<TResult>(methodName, nameof(methodName), genericTypeArguments, exactParameterMatch, args);
			if (result.Item2)
			{
				Mock.VerifyGet(mock, result.Item1, times, null);
			}
			else
			{
				Mock.Verify(mock, result.Item1, times, null);
			}
		}
		
		public void VerifyGet<TProperty>(string propertyName, Times times)
		{
			Mock.VerifyGet(mock, GetGetterExpression<TProperty>(propertyName), times, null);
		}

		public void VerifyGet(string indexerName, Times times, object[] indexerKeys)
		{
			var expression = GetIndexerGetterExpression(indexerName, indexerKeys);
			Mock.VerifyGet(mock, expression, times, null);
		}

		public void VerifySet(string propertyName, Times times, object value)
		{
			var expression = GetSetterExpressionIncludingIndexer(propertyName, value, false, nameof(propertyName));

			Mock.VerifySet(mock, expression, times, null);
		}

		public void VerifySet(string indexerName, Times times, object[] indexerKeys, object value)
		{
			var expression = GetIndexerSetterExpression(indexerName, indexerKeys, value);
			Mock.VerifySet(mock, expression, times, null);
		}

		public void VerifySet<TProperty>(string propertyName, Times times, object value)
		{
			var expression = GetSetterExpressionIncludingIndexer(propertyName, value, false, nameof(propertyName));
			Mock.VerifySet(mock, expression, times, null);
		}

		#endregion

		private static Expression<Func<T, TResult>> GetMemberAccess<TResult>(PropertyInfo property)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Func<T, TResult>>(Expression.MakeMemberAccess(param, property), param);
		}

		private static MethodInfo GetMethod(string methodName, Type[] genericTypeArguments, bool exact, params object[] args)
		{
			var argTypes = ToArgTypes(args);
			var methods = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(m => m.Name == methodName);
			if (genericTypeArguments != null && genericTypeArguments.Length > 0)
			{
				methods = methods
					.Where(m => m.IsGenericMethod && m.GetGenericArguments().Length == genericTypeArguments.Length)
					.Select(m => m.MakeGenericMethod(genericTypeArguments));
			}

			return methods
				.SingleOrDefault(m => m.GetParameterTypes().CompareTo(argTypes, exact, considerTypeMatchers: false));
		}

		private static Expression<Func<T, TResult>> GetMethodCall<TResult>(string methodName, string methodNameParameterName, Type[] genericTypeArguments,
			bool exactParameterMatch, object[] args)
		{
			Guard.NotNullOrEmpty(methodName, methodNameParameterName);

			var method = GetMethod(methodName, genericTypeArguments, exactParameterMatch, args);
			ThrowIfMethodMissing(methodName, method, args);
			ThrowIfVoidMethod(method);
			ThrowIfPublicMethod(method, typeof(T).Name);

			return GetMethodCallImpl<Func<T, TResult>>(method, args);
		}

		private static Expression<Action<T>> GetMethodCall(string methodName, string methodNameParameterName,Type[] genericTypeArguments, bool exactParameterMatch, params object[] args)
		{
			Guard.NotNullOrEmpty(methodName, methodNameParameterName);

			var method = GetMethod(methodName, genericTypeArguments, exactParameterMatch, args);
			ThrowIfMethodMissing(methodName, method, args);
			ThrowIfPublicMethod(method, typeof(T).Name);

			return GetMethodCallImpl<Action<T>>(method, args);
		}

		private static Expression<TDelegate> GetMethodCallImpl<TDelegate>(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<TDelegate>(Expression.Call(param, method, ToExpressionArgs(method, args)), param);
		}
		
		private static PropertyInfo GetProperty(string propertyName)
		{
			return typeof(T).GetProperty(
				propertyName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}

		private static Expression<Func<T, TProperty>> GetGetterExpression<TProperty>(string propertyName)
		{
			Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

			var property = GetProperty(propertyName);
			ThrowIfMemberMissing(propertyName, property);
			ThrowIfGetterNotApplicable(property);

			return GetMemberAccess<TProperty>(property);
		}

		private static Expression<Action<T>> GetSetterExpression(PropertyInfo property, Expression value)
		{
			var param = Expression.Parameter(typeof(T), "mock");

			return Expression.Lambda<Action<T>>(
				Expression.Call(param, property.GetSetMethod(true), value),
				param);
		}

		private static PropertyInfo GetPropertyIncludingIndexer(string propertyName, bool isSetter, object value, bool exact = true)
		{
			var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var matchingProperties = properties.Where(p => p.Name == propertyName).ToList();
			if (matchingProperties.Count > 1)
			{
				return GetMatchingIndexer(matchingProperties, CastValueForIndexer(value, propertyName), isSetter, exact);
			}
			if (matchingProperties.Count == 0)
			{
				return null;
			}
			return matchingProperties[0];
		}

		private static PropertyInfo GetMatchingIndexer(List<PropertyInfo> indexers, object[] keys, bool isSetter, bool exact)
		{
			var argTypes = ToArgTypes(keys);

			var matchingIndexer = indexers.FirstOrDefault(p =>
			{
				if (isSetter)
				{
					if (!p.CanWrite) { return false; }
					return p.GetSetMethod(true).GetParameterTypes().CompareTo(argTypes, exact, considerTypeMatchers: false);
				}
				else
				{
					if (!p.CanRead) { return false; }
					return p.GetGetMethod(true).GetParameterTypes().CompareTo(argTypes, exact, considerTypeMatchers: false);
				}

			});

			return matchingIndexer;
		}
		
		private static Expression<Action<T>> GetIndexerSetterExpression(string indexerName, object[] indexerKeys, object value)
		{
			Guard.NotNullOrEmpty(indexerName, nameof(indexerName));
			Guard.NotNullOrEmpty(indexerKeys, nameof(indexerKeys));

			var setupSetArguments = indexerKeys.Concat(new object[] { value }).ToArray();
			var indexer = GetPropertyIncludingIndexer(indexerName, true, setupSetArguments, false);
			ThrowIfMemberMissing(indexerName, indexer);
			Guard.IsIndexer(indexer, nameof(indexerName));
			ThrowIfSetterNotApplicable(indexer);

			return GetIndexerSetterExpression(indexer, setupSetArguments);
		}

		private static Expression<Action<T>> GetIndexerSetterExpression(PropertyInfo property, object[] indexerKeysAndValue)
		{
			var param = Expression.Parameter(typeof(T), "mock");

			return Expression.Lambda<Action<T>>(
				GetIndexerMethodCallExpression(param, property.GetSetMethod(true), indexerKeysAndValue),
				param);
		}

		private static LambdaExpression GetIndexerGetterExpression(string indexerName, object[] indexerKeys)
		{
			Guard.NotNullOrEmpty(indexerName, nameof(indexerName));
			Guard.NotNullOrEmpty(indexerKeys, nameof(indexerKeys));

			var indexer = GetPropertyIncludingIndexer(indexerName, false, indexerKeys, false);
			ThrowIfMemberMissing(indexerName, indexer);
			Guard.IsIndexer(indexer, nameof(indexerName));
			ThrowIfGetterNotApplicable(indexer);

			return GetIndexerGetterExpression(indexer, indexerKeys);
		}

		private static LambdaExpression GetIndexerGetterExpression(PropertyInfo property, object[] indexerKeys)
		{
			var param = Expression.Parameter(typeof(T), "mock");

			return Expression.Lambda(
				GetIndexerMethodCallExpression(param, property.GetGetMethod(true), indexerKeys),
				param
			);
		}

		private static MethodCallExpression GetIndexerMethodCallExpression(ParameterExpression mockParameter,MethodInfo accessorMethod, object[] indexerArguments)
		{
			var types = accessorMethod.GetParameterTypes();
			var expressionArgs = indexerArguments.Select((a, i) =>
			{
				return ToExpressionArg(types[i], indexerArguments[i]);
			}).ToArray();
			return Expression.Call(mockParameter, accessorMethod, expressionArgs);
		}
		
		private static LambdaExpression GetSetterExpressionIncludingIndexer(string propertyName, object value, bool exact, string propertyNameParameterName, bool throwIfMemberMissing = true)
		{
			Guard.NotNullOrEmpty(propertyName, propertyNameParameterName);

			var property = GetPropertyIncludingIndexer(propertyName, true, value, exact);
			if (property == null && !throwIfMemberMissing)
			{
				return null;
			}

			ThrowIfMemberMissing(propertyName, property);
			ThrowIfSetterNotApplicable(property);

			Expression<Action<T>> expression;
			if (property.GetIndexParameters().Length > 0)
			{
				expression = GetIndexerSetterExpression(property, CastValueForIndexer(value, propertyName));
			}
			else
			{
				expression = GetSetterExpression(property, ToExpressionArg(property.PropertyType, value));

			}
			return expression;
		}

		private static LambdaExpression GetSetterExpressionIncludingIndexerParams(string propertyName, object[] value, bool exact, string propertyNameParameterName, bool throwIfMemberMissing = true)
		{
			Guard.NotNullOrEmpty(propertyName, propertyNameParameterName);

			var property = GetPropertyIncludingIndexer(propertyName, true, value, exact);
			if (property == null && !throwIfMemberMissing)
			{
				return null;
			}

			ThrowIfMemberMissing(propertyName, property);
			ThrowIfSetterNotApplicable(property);

			Expression<Action<T>> expression;
			if (property.GetIndexParameters().Length > 0)
			{
				expression = GetIndexerSetterExpression(property, value);
			}
			else
			{
				expression = GetSetterExpression(property, ToExpressionArg(property.PropertyType, value[0]));

			}
			return expression;
		}

		private static LambdaExpression GetGetterExpressionIncludingIndexer<TResult>(string propertyName, object value, bool exact, string propertyNameParameterName)
		{
			Guard.NotNullOrEmpty(propertyName, propertyNameParameterName);
			LambdaExpression expression = null;
			var property = GetPropertyIncludingIndexer(propertyName, false, value, exact);
			if (property != null)
			{
				ThrowIfGetterNotApplicable(property);

				if (property.GetIndexParameters().Length > 0)
				{
					expression = GetIndexerGetterExpression(property, value as object[]);
				}
				else
				{
					expression = GetMemberAccess<TResult>(property);
				}
			}

			return expression;
		}

		private Tuple<LambdaExpression,bool> GetGetterIncludingIndexerOrMethodExpression<TResult>(string methodOrPropertyName, string methodOrPropertyNameParameterName,
			Type[] genericTypeArguments, bool exactParameterMatch, object[] args)
		{
			var isPropertyExpression = true;
			var expression = GetGetterExpressionIncludingIndexer<TResult>(methodOrPropertyName, args, exactParameterMatch, methodOrPropertyNameParameterName);
			if (expression == null)
			{
				isPropertyExpression = false;
				expression = GetMethodCall<TResult>(methodOrPropertyName, methodOrPropertyNameParameterName, genericTypeArguments, exactParameterMatch, args);
			}
			return new Tuple<LambdaExpression, bool>(expression, isPropertyExpression);
		}

		private void GetSetterIncludingIndexerOrMethodExpression(string methodOrPropertyName, string methodOrPropertyNameParameterName,
			Type[] genericTypeArguments, bool exactParameterMatch, object[] args, Action<LambdaExpression> propertyExpressionCallback, Action<Expression<Action<T>>> methodExpressionCallback)
		{
			var propertyExpression = GetSetterExpressionIncludingIndexerParams(methodOrPropertyName, args, exactParameterMatch, methodOrPropertyNameParameterName, false);
			if (propertyExpression != null)
			{
				propertyExpressionCallback(propertyExpression);
			}
			else
			{
				var methodCall = GetMethodCall(methodOrPropertyName, methodOrPropertyNameParameterName, genericTypeArguments, exactParameterMatch, args);
				methodExpressionCallback(methodCall);
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

		private static void ThrowIfMethodMissing(string methodName, MethodInfo method, object[] args)
		{
			if (method == null)
			{
				List<string> extractedTypeNames = new List<string>();
				foreach (object o in args)
				{
					if (o is Expression expr)
					{
						extractedTypeNames.Add(expr.Type.GetFormattedName());
					}
					else
					{
						extractedTypeNames.Add(o.GetType().GetFormattedName());
					}
				}

				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MethodMissing,
					typeof(T).Name,
					methodName,
					string.Join(
						", ",
						extractedTypeNames.ToArray())));
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
			if (property.CanRead(out var getter) && getter.IsPublic)
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
			if (property.CanWrite(out var setter) && setter.IsPublic)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnexpectedPublicProperty,
					reflectedTypeName,
					property.Name));
			}
		}
		
		private static void ThrowIfGetterNotApplicable(PropertyInfo property)
		{
			ThrowIfPublicGetter(property, typeof(T).Name);
			Guard.CanRead(property);
		}

		private static void ThrowIfSetterNotApplicable(PropertyInfo property)
		{
			ThrowIfPublicSetter(property, typeof(T).Name);
			Guard.CanWrite(property);
		}

		private static void ThrowIfVoidMethod(MethodInfo method)
		{
			if (method.ReturnType == typeof(void))
			{
				throw new ArgumentException(Resources.CantSetReturnValueForVoid);
			}
		}

		private static object[] CastValueForIndexer(object value,string propertyName)
		{
			if (value is object[])
			{
				return value as object[];
			}

			throw new ArgumentException($"{propertyName} is an indexer but value is not an object array");
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
				else if (ItRefAnyField(expr) is FieldInfo itRefAnyField)
				{
					types[index] = itRefAnyField.FieldType.MakeByRefType();
				}
				else if (expr.NodeType == ExpressionType.MemberAccess)
				{
					var member = (MemberExpression)expr;
					if (member.Member is FieldInfo field)
					{
						types[index] = field.FieldType;
					}
					else if (member.Member is PropertyInfo property)
					{
						types[index] = property.PropertyType;
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
					types[index] = (expr.PartialEval() as ConstantExpression)?.Type;
				}
			}

			return types;
		}

		private static bool IsItRefAny(Expression expression)
		{
			return ItRefAnyField(expression) != null;
		}

		private static FieldInfo ItRefAnyField(Expression expr)
		{
			FieldInfo itRefAnyField = null;

			if (expr.NodeType == ExpressionType.MemberAccess)
			{
				var member = (MemberExpression)expr;
				if (member.Member is FieldInfo field)
				{
					if (field.Name == nameof(It.Ref<object>.IsAny))
					{
						var fieldDeclaringType = field.DeclaringType;
						if (fieldDeclaringType.IsGenericType)
						{
							var fieldDeclaringTypeDefinition = fieldDeclaringType.GetGenericTypeDefinition();
							if (fieldDeclaringTypeDefinition == typeof(It.Ref<>))
							{
								itRefAnyField = field;
							}
						}
					}
				}
			}

			return itRefAnyField;
		}

		private static Expression ToExpressionArg(Type type, object arg)
		{
			if (arg is Expression expression)
			{
				if (!type.IsAssignableFrom(expression.GetType()))
				{
					if(arg is LambdaExpression lambda)
					{
						expression = lambda.Body;
					}
					return expression;
				}
				
				if (IsItRefAny(expression))
				{
					return expression;
				}

				if (expression.IsMatch(out _))
				{
					return expression;
				}

			}

			return Expression.Constant(arg, type);
		}

		private static IEnumerable<Expression> ToExpressionArgs(MethodInfo method, object[] args)
		{
			ParameterInfo[] methodParams = method.GetParameters();
			for (int i = 0; i < args.Length; i++)
			{
				yield return ToExpressionArg(methodParams[i].ParameterType, args[i]);
			}
		}
	}
}
