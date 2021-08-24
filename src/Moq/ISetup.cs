// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		// NOTE regarding the `InnerMock` and `InnerMocks` properties below:
		//
		// Moq has support for multi-method setups, but does not yet make actual use of them.
		// Once that changes, some setups may have more than one inner mock. This possibility
		// has already been accounted for in Moq's internals, but not in this interface:
		// The `InnerMock` property should really be replaced with an `InnerMocks` one.
		//
		// Before making this change, let's wait a little longer to see whether it can be avoided.
		// This is a distinct possibility since fixing Moq's recursive verification algorithm
		// may do away with the need to expose the "inner mock" concept altogether. It would be
		// a pity to add `InnerMocks`, only to have both properties become obsolete soon after.

		/// <summary>
		///   Gets the inner mock of this setup (if present and known).
		///   <para>
		///     An "inner mock" is the <see cref="Moq.Mock"/> instance associated with a setup's return value,
		///     if that setup is configured to return a mock object.
		///   </para>
		///   <para>
		///     This property will be <see langword="null"/> if a setup either does not return a mock object,
		///     or if Moq cannot safely determine its return value without risking any side effects. For instance,
		///     Moq is able to inspect the return value if it is a constant (e.g. <c>`.Returns(value)`</c>);
		///     if, on the other hand, it gets computed by a factory function (e.g. <c>`.Returns(() => value)`</c>),
		///     Moq will not attempt to retrieve that value just to find the inner mock,
		///     since calling a user-provided function could have effects beyond Moq's understanding and control.
		///   </para>
		/// </summary>
		// /// <exception cref="InvalidOperationException">The setup has more than one inner mock.</exception>
		// [Obsolete("Use 'InnerMocks' instead.")]
		// [EditorBrowsable(EditorBrowsableState.Never)]
		Mock InnerMock { get; }

		// /// <summary>
		// ///   Gets the inner mocks of this setup (if present and known).
		// ///   <para>
		// ///     An "inner mock" is the <see cref="Moq.Mock"/> instance associated with a setup's return value,
		// ///     if that setup is configured to return a mock object.
		// ///   </para>
		// ///   <para>
		// ///     This property will return an empty sequence if a setup either does not return any mock objects,
		// ///     or if Moq cannot safely determine its return value(s) without risking any side effects. For instance,
		// ///     Moq is able to inspect the return value if it is a constant (e.g. <c>`.Returns(value)`</c>);
		// ///     if, on the other hand, it gets computed by a factory function (e.g. <c>`.Returns(() => value)`</c>),
		// ///     Moq will not attempt to retrieve that value just to find the inner mock,
		// ///     since calling a user-provided function could have effects beyond Moq's understanding and control.
		// ///   </para>
		// /// </summary>
		// IEnumerable<Mock> InnerMocks { get; }

		/// <summary>
		///   Gets whether this setup is conditional.
		/// </summary>
		/// <seealso cref="Mock{T}.When(Func{bool})"/>
		bool IsConditional { get; }

		/// <summary>
		///   Gets whether this setup was matched by at least one invocation on the mock.
		/// </summary>
		bool IsMatched { get; }

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
		///   Returns the original setup expression from which this setup resulted.
		///   <para>
		///     For setups doing a simple member access or method invocation (such as <c>`mock => mock.Member`</c>),
		///     this property will be equal to <see cref="Expression"/>.
		///   </para>
		///   <para>
		///     For setups whose expression involves member chaining (such as <c>`parent => parent.Child.Member`</c>),
		///     Moq does not create a single setup, but one for each member access/invocation.
		///     The example just given will result in two setups:
		///     <list type="number">
		///       <item>a setup for <c>`parent => parent.Child`</c> on the parent mock; and</item>
		///       <item>on its inner mock, a setup for <c>`(child) => (child).Member`</c>.</item>
		///     </list>
		///     These are the setups that will be put in the mocks' <see cref="Mock.Setups"/> collections;
		///     their <see cref="Expression"/> will return the partial expression for just a single member access,
		///     while their <see cref="OriginalExpression"/> will return the original, full expression.
		///   </para>
		///   <para>
		///     This property may also return <see langword="null"/> if this setup was created automatically,
		///     e.g. by <see cref="Mock{T}.SetupAllProperties"/> or by <see cref="DefaultValue.Mock"/>.
		///   </para>
		/// </summary>
		Expression OriginalExpression { get; }

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
