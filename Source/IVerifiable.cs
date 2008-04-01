
namespace Moq
{
	/// <devdoc>
	/// Internal interface implemented by Mock{T} to enable 
	/// <see cref="MockFactory"/> to verify mocks in a generic way.
	/// </devdoc>
	internal interface IVerifiable
	{
		void Verify();
		void VerifyAll();
	}
}
