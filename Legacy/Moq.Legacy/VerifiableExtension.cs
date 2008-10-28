using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;
using Moq.Language;

namespace Moq
{
	public static class VerifiableExtension
	{
		/// <summary>
		/// Marks the expectation as verifiable, meaning that a call 
		/// to <see cref="Mock{T}.Verify()"/> will check if this particular 
		/// expectation was met.
		/// </summary>
		/// <example>
		/// The following example marks the expectation as verifiable:
		/// <code>
		/// mock.Expect(x => x.Execute("ping"))
		///     .Returns(true)
		///     .Verifiable();
		/// </code>
		/// </example>
		public static void Verifiable(this IReturnsResult mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}

		public static void Verifiable(this IThrowsResult mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}

		public static void Verifiable(this IExtensible mock)
		{
			((MethodCall)mock).IsVerifiable = true;
		}
	}
}
