using System.Reflection;

namespace Moq.Proxy.Factory
{
	/// <summary>
	/// Defines a contract that represents a method call.
	/// </summary>
	public interface IMethodCall
	{
		/// <summary>
		/// Gets the input arguments.
		/// </summary>
		/// <value>The input arguments.</value>
		object[] InArgs { get; }

		/// <summary>
		/// Gets the instance of the target.
		/// </summary>
		/// <value>The instance of the target.</value>
		object Instance { get; }

		/// <summary>
		/// Gets the method called.
		/// </summary>
		/// <value>The method called.</value>
		MethodInfo Method { get; }
	}
}