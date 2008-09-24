using System;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language.Flow;
using Moq.Protected;

namespace Moq
{
	internal class SetterMethodCall<TProperty> : MethodCall, IExpectSetter<TProperty>
	{
		public SetterMethodCall(Expression originalExpression, MethodInfo method)
			: base(originalExpression, method, new[] { ItExpr.IsAny<TProperty>() })
		{
		}

		public SetterMethodCall(Expression originalExpression, MethodInfo method, TProperty value)
			: base(originalExpression, method, new[] { ItExpr.Is<TProperty>(arg => Object.Equals(arg, value)) })
		{
		}

		public ICallbackResult Callback(Action<TProperty> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}
	}
}
