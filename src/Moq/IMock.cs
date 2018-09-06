// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics.CodeAnalysis;

namespace Moq
{
	/// <summary>
	/// Covariant interface for <see cref="Mock{T}"/> such that casts between IMock&lt;Employee&gt; to IMock&lt;Person&gt;
	/// are possible. Only covers the covariant members of <see cref="Mock{T}"/>.
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
