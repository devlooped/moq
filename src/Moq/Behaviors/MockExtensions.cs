// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Moq.Behaviors.Language;

namespace Moq.Behaviors
{
	/// <summary>
	///   Extension methods for creating <see cref="Behavior"/>-driven setups.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockExtensions
	{
		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for the given expression.
		///   <para>
		///     This method offers practically no static type safety and shouldn't be casually used in regular test code.
		///     It is intended as an advanced entry point into Moq's setup machinery for those developing extensions
		///     beyond the fluent setup API.
		///   </para>
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="expression">
		///   The expression describing the invocation(s) for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static IBehaviorSetupResult Setup(this Mock mock, LambdaExpression expression, IEnumerable<Behavior> behaviors)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var parts = expression.Split();

			var setup = Mock.SetupRecursive(mock, expression, setupLast: (targetMock, originalExpression, part) =>
			{
				var lastSetup = new BehaviorSetup(originalExpression, targetMock, part, behaviors.ToArray());
				targetMock.MutableSetups.Add(lastSetup);
				return lastSetup;
			});

			return new BehaviorSetupPhrase(setup);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of a <see langword="void"/> method.
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="action">
		///   The expression describing the invocation(s) of a <see langword="void"/> method for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult Setup<T>(this Mock<T> mock, Expression<Action<T>> action, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(action, nameof(action));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)action, behaviors);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of a non-<see langword="void"/> method or property.
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="expression">
		///   The expression describing the invocation(s) of a non-<see langword="void"/> method or property for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult Setup<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of an event <see langword="add"/> accessor (<c>`+=`</c>).
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="addExpression">
		///   The expression describing the invocation(s) of an event <see langword="add"/> accessor for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult SetupAdd<T>(this Mock<T> mock, Action<T> addExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(addExpression, nameof(addExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, mock.ConstructorArguments);
			Guard.IsEventAdd(expression, nameof(expression));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of a property getter.
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="getterExpression">
		///   The expression describing the invocation(s) of a property getter for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult SetupGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> getterExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(getterExpression, nameof(getterExpression));
			Guard.IsPropertyOrIndexer(getterExpression, nameof(getterExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)getterExpression, behaviors);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of an event <see langword="remove"/> accessor (<c>`-=`</c>).
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="removeExpression">
		///   The expression describing the invocation(s) of an event <see langword="remove"/> accessor for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult SetupRemove<T>(this Mock<T> mock, Action<T> removeExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, mock.ConstructorArguments);
			Guard.IsEventRemove(expression, nameof(expression));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <summary>
		///   Creates a <see cref="Behavior"/>-driven setup for invocation(s) of a property setter.
		/// </summary>
		/// <param name="mock">
		///   The mock on which to create a setup.
		/// </param>
		/// <param name="setterExpression">
		///   The expression describing the invocation(s) of a property setter for which to create a setup.
		/// </param>
		/// <param name="behaviors">
		///   A set of <see cref="Behavior"/> defining how the setup will react to invocations.
		/// </param>
		public static IBehaviorSetupResult SetupSet<T, TProperty>(this Mock<T> mock, Action<T> setterExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, mock.ConstructorArguments);
			Guard.IsAssignmentToPropertyOrIndexer(expression, nameof(expression));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}
	}
}
