using System;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language.Flow;
using Moq.Protected;

namespace Moq
{
	internal class SetterMethodCall<TMock, TProperty> : MethodCall<TMock>, ISetupSetter<TMock, TProperty>
	{
		public SetterMethodCall(Mock mock, Expression originalExpression, MethodInfo method)
			: base(mock, originalExpression, method, new[] { ItExpr.IsAny<TProperty>() })
		{
		}

		public SetterMethodCall(Mock mock, Expression originalExpression, MethodInfo method, TProperty value)
			: base(mock, originalExpression, method, new[] { ItExpr.Is<TProperty>(arg => Object.Equals(arg, value)) })
		{
		}

		public SetterMethodCall(Mock mock, Expression originalExpression, MethodInfo method, Expression value)
			: base(mock, originalExpression, method, new[] { value })
		{
		}

		public ICallbackResult Callback(Action<TProperty> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}
	}
}
