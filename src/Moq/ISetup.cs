// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	///   A setup configured on a mock.
	/// </summary>
	/// <seealso cref="Mock.Setups"/>
	public interface ISetup
	{
		/// <summary>
		///   The setup expression.
		/// </summary>
		LambdaExpression Expression { get; }

		/// <summary>
		///   Gets whether this setup is conditional.
		/// </summary>
		/// <seealso cref="Mock{T}.When(Func{bool})"/>
		bool IsConditional { get; }

		/// <summary>
		///   Gets whether this setup has been overridden
		///   (that is, whether it is being shadowed by a more recent non-conditional setup with an equal expression).
		/// </summary>
		bool IsOverridden { get; }

		/// <summary>
		///   Gets whether this setup is "verifiable".
		/// </summary>
		/// <remarks>
		///   This property gets sets by the <c>`.Verifiable()`</c> setup verb.
		///   <para>
		///     Note that setups can be verified even if this property is <see langword="false"/>:
		///     <see cref="Mock.VerifyAll()"/> completely ignores this property.
		///     <see cref="Mock.Verify()"/>, however, will only verify setups where this property is <see langword="true"/>.
		///   </para>
		/// </remarks>
		bool IsVerifiable { get; }

		/// <summary>
		///   Gets whether this setup was matched by at least one invocation on the mock.
		/// </summary>
		bool WasMatched { get; }

		/// <summary>
		///   Verifies this setup and optionally all verifiable setups of its inner mock (if present and known).
		///   <para>
		///     If <paramref name="recursive"/> is set to <see langword="true"/>,
		///     the semantics of this method are essentially the same as those of <see cref="Mock.Verify()"/>,
		///     except that this setup (instead of a mock) is used as the starting point for verification,
		///     and will always be verified itself (even if not flagged as verifiable).
		///   </para>
		/// </summary>
		/// <param name="recursive">
		///   Specifies whether recursive verification should be performed.
		/// </param>
		/// <exception cref="MockException">
		///   Verification failed due to one or more unmatched setups.
		/// </exception>
		/// <seealso cref="VerifyAll()"/>
		/// <seealso cref="Mock.Verify()"/>
		void Verify(bool recursive = true);

		/// <summary>
		///   Verifies this setup and all setups of its inner mock (if present and known),
		///   regardless of whether they have been flagged as verifiable.
		///   <para>
		///     The semantics of this method are essentially the same as those of <see cref="Mock.VerifyAll()"/>,
		///     except that this setup (instead of a mock) is used as the starting point for verification.
		///   </para>
		/// </summary>
		/// <exception cref="MockException">
		///   Verification failed due to one or more unmatched setups.
		/// </exception>
		/// <seealso cref="Verify(bool)"/>
		/// <seealso cref="Mock.VerifyAll()"/>
		void VerifyAll();
	}
}
