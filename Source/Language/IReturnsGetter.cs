using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Returns</c> verb for property get expectations.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsGetter<TProperty> : IReturns, IHideObjectMembers
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the property getter call:
		/// <code>
		/// mock.ExpectGet(x => x.Suspended)
		///     .Returns(true);
		/// </code>
		/// </example>
		IOnceVerifies Returns(TProperty value);
		/// <summary>
		/// Specifies a function that will calculate the value to return for the property.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example>
		/// Return a calculated value when the property is retrieved:
		/// <code>
		/// mock.ExpectGet(x => x.Suspended)
		///     .Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the property  
		/// is retrieved and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IOnceVerifies Returns(Func<TProperty> valueFunction);
	}
}
