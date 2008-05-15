
namespace Moq.Protected
{
	/// <summary>
	/// Enables the <c>Protected()</c> method on <see cref="Mock{T}"/>, 
	/// allowing expectations to be set for protected members by using their 
	/// name as a string, rather than strong-typing them which is not possible 
	/// due to their visibility.
	/// </summary>
	public static class ProtectedExtension
	{
		/// <summary>
		/// Enable protected expectations for the mock.
		/// </summary>
		/// <typeparam name="T">Mocked object type. Typically omitted as it can be inferred from the mock instance.</typeparam>
		/// <param name="mock">The mock to set the protected expectations on.</param>
		public static IProtectedMock Protected<T>(this Mock<T> mock)
			where T : class
		{
			Guard.ArgumentNotNull(mock, "mock");

			return new ProtectedMock<T>(mock);
		}
	}
}
