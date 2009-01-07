using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;
using Moq.Language;

namespace Moq
{
	/// <summary>
	/// Adds <c>Verifiable</c> verb to mock setups.
	/// </summary>
	public static class VerifiableExtension
	{
		/// <summary>
		/// Marks the setup as verifiable, meaning that a call 
		/// to <c>mock.Verify()</c> will check if this particular 
		/// setup was matched.
		/// </summary>
		/// <example>
		/// The following example marks the setup as verifiable:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(true)
		///     .Verifiable();
		/// </code>
		/// </example>
		public static void Verifiable(this IReturnsResult mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}

		/// <summary>
		/// Marks the setup as verifiable, meaning that a call 
		/// to <c>mock.Verify()</c> will check if this particular 
		/// setup was matched.
		/// </summary>
		/// <example>
		/// The following example marks the setup as verifiable:
		/// <code>
		/// mock.Setup(x => x.Execute(""))
		///     .Throws(new ArgumentException())
		///     .Verifiable();
		/// </code>
		/// </example>
		public static void Verifiable(this IThrowsResult mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}

		/// <summary>
		/// Marks the setup as verifiable, meaning that a call 
		/// to <c>mock.Verify()</c> will check if this particular 
		/// setup was matched.
		/// </summary>
		/// <example>
		/// The following example marks the setup as verifiable:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(true)
		///     .Verifiable();
		/// </code>
		/// </example>
		public static void Verifiable(this IExtensible mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}
	}
}
