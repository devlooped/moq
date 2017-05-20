using System.Reflection;

namespace Moq.Proxy
{
	/// <summary>
	/// Class that represents the method call currently being executed. 
	/// Allows for custom interception for open generic methods
	/// </summary>
	public interface IPublicCallContext
	{
		/// <summary>
		/// Arguments passed into the method
		/// </summary>
		object[] Arguments { get; }

		/// <summary>
		/// Closed generic method currently being executed.
		/// </summary>
		MethodInfo Method { get; }
	}
}