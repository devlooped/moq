using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq
{
	public static class StubExtensions
	{
		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property)
			 where T : class
		{
			mock.Stub(property, default(TProperty));
		}

		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property, TProperty defaultValue)
			 where T : class
		{
			TProperty value = defaultValue;
			mock.ExpectGet(property).Returns(() => value);
			mock.ExpectSet(property).Callback(p => value = p);
		}
	}
}
