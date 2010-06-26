using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetupConditionResult<TMock> where TMock : class
	{
		/// <summary>
		/// The expectation will be considered only in the former condition.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		ISetup<TMock> Setup(Expression<Action<TMock>> expression);

		/// <summary>
		/// The expectation will be considered only in the former condition.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		ISetup<TMock, TResult> Setup<TResult>(Expression<Func<TMock, TResult>> expression);
	}
}