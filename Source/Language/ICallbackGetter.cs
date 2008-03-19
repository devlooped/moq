using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb for property getter expectations.
	/// </summary>
	/// <seealso cref="Mock{T}.ExpectGet"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallbackGetter<TProperty> : IHideObjectMembers
	{ 
		/// <summary>
		/// Specifies a callback to invoke when the property is retrieved.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the property value being set. 
		/// <code>
		/// mock.ExpectGet(x => x.Suspended)
		///     .Callback(() => called = true)
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrowsGetter<TProperty> Callback(Action callback);
	}
}
