using System.ComponentModel;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Interface to be implemented by classes that determine the 
	/// default value of non-expected invocations.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal interface IDefaultValueProvider
	{
		/// <summary>
		/// Provides a value for the given member and arguments.
		/// </summary>
		/// <param name="member">The member to provide a default 
		/// value for.</param>
		/// <param name="arguments">Optional arguments passed in 
		/// to the call that requires a default value.</param>
		object ProvideDefault(MethodInfo member, object[] arguments);
	}
}
