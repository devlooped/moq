// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	///   A setup configured on a mock.
	/// </summary>
	public interface ISetup
	{
		/// <summary>
		///   The setup expression.
		/// </summary>
		LambdaExpression Expression { get; }

		/// <summary>
		///   Gets whether this setup is conditional.
		/// </summary>
		bool IsConditional { get; }

		/// <summary>
		///   Gets whether this setup is disabled.
		/// </summary>
		bool IsDisabled { get; }

		/// <summary>
		///   Gets whether this setup was matched by at least one invocation on the mock.
		/// </summary>
		bool IsMatched { get; }

		/// <summary>
		///   Gets whether this setup is verifiable.
		/// </summary>
		bool IsVerifiable { get; }

		/// <summary>
		///   Checks whether this setup is a part of a composite setup.
		///   If so, the composite setup of which this one is a part is returned via the <see langword="out"/> parameter <paramref name="originalSetup"/>.
		///   <para>
		///     (Composite setups are those with expressions where several member accesses or calls are chained together.
		///     They are a much more accurate reflection of the original setup expressions you will find in your own code,
		///     whereas the setups returned by <see cref="Mock.Setups"/> are more "technical" in nature:
		///     They only reflect the part of an original setup expression that is of immediate concern to the mock.)
		///   </para>
		/// </summary>
		/// <param name="originalSetup">
		///   If this setup is part of a composite setup,
		///   this <see langword="out"/> parameter will be set to the latter.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if this setup is a part of a composite setup;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		/// <seealso cref="ICompositeSetup"/>
		/// <seealso cref="ReturnsInnerMock(out Mock)"/>
		bool IsPartOfCompositeSetup(out ICompositeSetup originalSetup);

		/// <summary>
		///   Checks whether this setup is known to return a mocked object.
		///   If so, the corresponding <see cref="Mock"/> (the so-called "inner mock") is returned via the <see langword="out"/> parameter <paramref name="innerMock"/> instance.
		/// </summary>
		/// <param name="innerMock">
		///   If this setup is known to return a mocked object,
		///   this <see langword="out"/> parameter will be set to the corresponding <see cref="Mock"/> instance.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if this setup is known to return a mocked object;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		bool ReturnsInnerMock(out Mock innerMock);

		/// <summary>
		///   Verifies this setup.
		///   That is, checks whether it has been matched by at least one invocation on the mock.
		///   <para>
		///     If this setup is known to return an inner mock object, the inner mock will also be verified.
		///   </para>
		/// </summary>
		/// <exception cref="MockException">
		///   The setup or its inner mock did not verify successfully.
		/// </exception>
		/// <seealso cref="ReturnsInnerMock(out Mock)"/>
		/// <seealso cref="Mock.Verify()"/>
		/// <seealso cref="Mock.VerifyAll()"/>
		void Verify();
	}
}
