using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Adds <c>Stub</c> extension method to a mock so that you can 
	/// stub properties.
	/// </summary>
	public static class StubExtensions
	{
		/// <summary>
		/// Specifies that the given property should have stub behavior, 
		/// meaning that setting its value will cause it to be saved and 
		/// later returned when the property is requested.
		/// </summary>
		/// <typeparam name="T">Mocked type, inferred from the object 
		/// where this method is being applied (does not need to be specified).</typeparam>
		/// <typeparam name="TProperty">Type of the property, inferred from the property 
		/// expression (does not need to be specified).</typeparam>
		/// <param name="mock">The instance to stub.</param>
		/// <param name="property">Property expression to stub.</param>
		/// <example>
		/// If you have an interface with an int property <c>Value</c>, you might 
		/// stub it using the following straightforward call:
		/// <code>
		/// var mock = new Mock&lt;IHaveValue&gt;();
		/// mock.Stub(v => v.Value);
		/// </code>
		/// After the <c>Stub</c> call has been issued, setting and 
		/// retrieving the object value will behave as expected:
		/// <code>
		/// IHaveValue v = mock.Object;
		/// 
		/// v.Value = 5;
		/// Assert.Equal(5, v.Value);
		/// </code>
		/// </example>
		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property)
			 where T : class
		{
			mock.Stub(property, default(TProperty));
		}

		/// <summary>
		/// Specifies that the given property should have stub behavior, 
		/// meaning that setting its value will cause it to be saved and 
		/// later returned when the property is requested. This overload 
		/// allows setting the initial value for the property.
		/// </summary>
		/// <typeparam name="T">Mocked type, inferred from the object 
		/// where this method is being applied (does not need to be specified).</typeparam>
		/// <typeparam name="TProperty">Type of the property, inferred from the property 
		/// expression (does not need to be specified).</typeparam>
		/// <param name="mock">The instance to stub.</param>
		/// <param name="property">Property expression to stub.</param>
		/// <param name="initialValue">Initial value for the property.</param>
		/// <example>
		/// If you have an interface with an int property <c>Value</c>, you might 
		/// stub it using the following straightforward call:
		/// <code>
		/// var mock = new Mock&lt;IHaveValue&gt;();
		/// mock.Stub(v => v.Value, 5);
		/// </code>
		/// After the <c>Stub</c> call has been issued, setting and 
		/// retrieving the object value will behave as expected:
		/// <code>
		/// IHaveValue v = mock.Object;
		/// // Initial value was stored
		/// Assert.Equal(5, v.Value);
		/// 
		/// // New value set which changes the initial value
		/// v.Value = 6;
		/// Assert.Equal(6, v.Value);
		/// </code>
		/// </example>
		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property, TProperty initialValue)
			 where T : class
		{
			TProperty value = initialValue;
			mock.ExpectGet(property).Returns(() => value);
			mock.ExpectSet(property).Callback(p => value = p);
		}
	}
}
