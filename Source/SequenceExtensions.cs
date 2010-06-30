using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Moq.Language.Flow;
using Moq.Language;
using System.Diagnostics.CodeAnalysis;

namespace Moq
{
	/// <summary>
	/// Helper for sequencing return values in the same method.
	/// </summary>
	public static class SequenceExtensions
	{
		/// <summary>
		/// Return a sequence of values, once per call.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public static ISetupSequentialResult<TResult> SetupSequence<TMock, TResult>(
			this Mock<TMock> mock,
			Expression<Func<TMock, TResult>> expression)
			where TMock : class
		{
			return new SetupSequentialContext<TMock, TResult>(mock, expression);
		}
	}
}