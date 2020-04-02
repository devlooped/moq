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
		///   Returns the <see cref="Mock"/> instance to which this setup belongs.
		/// </summary>
		Mock Mock { get; }

		/// <summary>
		///   Gets whether this setup was matched by at least one invocation on the mock.
		/// </summary>
		bool WasMatched { get; }

		/// <summary>
		///   Gets whether this setup is part of a "fluent" setup
		///   (that is, one with a setup expression involving member chaining).
		///   If so, the fluent setup of which this one is a part is returned via the <see langword="out"/> parameter <paramref name="fluentSetup"/>.
		/// </summary>
		/// <param name="fluentSetup">
		///   If this setup is part of a fluent setup,
		///   this <see langword="out"/> parameter will be set to the latter.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if this setup is part of a fluent setup;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		/// <seealso cref="IFluentSetup"/>
		bool IsPartOfFluentSetup(out IFluentSetup fluentSetup);

		/// <summary>
		///   Gets whether this setup returns a mock object.
		///   If so, the corresponding <see cref="Mock"/> instance is returned via the <see langword="out"/> parameter <paramref name="innerMock"/>.
		/// </summary>
		/// <param name="innerMock">
		///   If this setup returns a mock object,
		///   this <see langword="out"/> parameter will be set to the corresponding <see cref="Mock"/> instance.
		/// </param>
		/// <returns>
		///   <list type="bullet">
		///     <item><see langword="true"/> if this setup returns a mock object;</item>
		///     <item><see langword="false"/> if it does not return a mock object;</item>
		///     <item><see langword="null"/> if the return value cannot be determined without risking any side effects.</item>
		///   </list>
		/// </returns>
		bool? ReturnsMock(out Mock innerMock);

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
