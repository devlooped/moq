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
		/// <summary>
		///   Exposes the mocked object instance.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		T Object { get; }

		/// <summary>
		///   Behavior of the mock, according to the value set in the constructor.
		/// </summary>
		MockBehavior Behavior { get; }

		/// <summary>
		///   Whether the base member virtual implementation will be called for mocked classes if no setup is matched.
		///   Defaults to <see langword="false"/>.
		/// </summary>
		bool CallBase { get; set; }

		/// <summary>
		///   Specifies the behavior to use when returning default values for unexpected invocations on loose mocks.
		/// </summary>
		DefaultValue DefaultValue { get; set; }
	}
}
