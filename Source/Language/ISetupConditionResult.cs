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
	public interface ISetupConditionResult<T> where T : class
	{
		/// <summary>
		/// The expectation will be considered only in the former condition.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		ISetup<T> Setup(Expression<Action<T>> expression);

		/// <summary>
		/// The expectation will be considered only in the former condition.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression);

		/// <summary>
		/// Setups the get.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<T, TProperty>> expression);

		/// <summary>
		/// Setups the set.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="setterExpression">The setter expression.</param>
		/// <returns></returns>
		ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression);

		/// <summary>
		/// Setups the set.
		/// </summary>
		/// <param name="setterExpression">The setter expression.</param>
		/// <returns></returns>
		ISetup<T> SetupSet(Action<T> setterExpression);
	}
}