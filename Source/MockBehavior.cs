
namespace Moq
{
	/// <summary>
	/// Options to customize the behavior of the mock. 
	/// </summary>
	public enum MockBehavior
	{
		/// <summary>
		/// Causes the mock to always throw 
		/// an exception for invocations that don't have a 
		/// corresponding expectation.
		/// </summary>
		Strict, 
		/// <summary>
		/// Matches the behavior of classes and interfaces 
		/// in equivalent manual mocks: abstract methods 
		/// need to have an expectation (override), as well 
		/// as all interface members. Other members (virtual 
		/// and non-virtual) can be called freely and will end up 
		/// invoking the implementation on the target type if available.
		/// </summary>
		Normal,
		/// <summary>
		/// Will only throw exceptions for abstract methods and 
		/// interface members which need to return a value and 
		/// don't have a corresponding expectation.
		/// </summary>
		Relaxed,
		/// <summary>
		/// Will never throw exceptions, returning default  
		/// values when necessary (null for reference types 
		/// or zero for value types).
		/// </summary>
		Loose,
		/// <summary>
		/// Default mock behavior, which equals <see cref="Normal"/>.
		/// </summary>
		Default = Normal,
	}
}
