using System;
using System.Linq.Expressions;

namespace Moq
{
	// Keeps legacy implementations.
	public partial class Mock
	{
		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock mock,
			Expression<Func<T1, TProperty>> expression, TProperty value)
			where T1 : class
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();
			ThrowIfPropertyNotWritable(prop);

			var setter = prop.GetSetMethod();
			ThrowIfCantOverride(expression, setter);

			var call = new SetterMethodCall<T1, TProperty>(mock, expression, setter, value);
			var targetInterceptor = GetInterceptor(expression, mock);

			targetInterceptor.AddCall(call, ExpectKind.PropertySet);

			return call;
		}

	}
}
