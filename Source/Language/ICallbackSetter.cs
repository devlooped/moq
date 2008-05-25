using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb for property setter expectations.
	/// </summary>
	/// <seealso cref="Mock{T}.ExpectSet"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallbackSetter<TProperty> : IHideObjectMembers
	{ 
		/// <summary>
		/// Specifies a callback to invoke when the property is set that receives the 
		/// property value being set.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the property value being set. 
		/// <code>
		/// mock.ExpectSet(x => x.Suspended)
		///     .Callback((bool state) => Console.WriteLine(state));
		/// </code>
		/// </example>
		ICallbackResult Callback(Action<TProperty> callback);
	}
}
