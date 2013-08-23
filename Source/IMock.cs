using System.Diagnostics.CodeAnalysis;

namespace Moq
{
	/// <summary>
	/// Covarient interface for Mock&lt;T&gt; such that casts between IMock&lt;Employee&gt; to IMock&lt;Person&gt;
	/// are possible. Only covers the covariant members of Mock&lt;T&gt;.
	/// </summary>
	public interface IMock<out T> where T : class
	{
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Object"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		T Object { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Behavior"]/*'/>
		MockBehavior Behavior { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.CallBase"]/*'/>
		bool CallBase { get; set; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.DefaultValue"]/*'/>
		DefaultValue DefaultValue { get; set; }
	}
}