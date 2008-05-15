using Moq.Language.Flow;

namespace Moq.Protected
{
	/// <summary>
	/// Allows expectations to be set for protected members by using their 
	/// name as a string, rather than strong-typing them which is not possible 
	/// due to their visibility.
	/// </summary>
	public interface IProtectedMock : IHideObjectMembers
	{
		/// <summary/>
		IExpect Expect(string voidMethodName, params object[] args);
		
		/// <summary/>
		IExpect<TResult> Expect<TResult>(string methodOrPropertyName, params object[] args);

		/// <summary/>
		IExpectGetter<TProperty> ExpectGet<TProperty>(string propertyName/*, params object[] args*/);

		/// <summary/>
		IExpectSetter<TProperty> ExpectSet<TProperty>(string propertyName/*, params object[] args*/);
	}
}
