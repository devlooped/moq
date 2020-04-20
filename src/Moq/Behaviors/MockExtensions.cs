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
	/// <todo/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockExtensions
	{
		/// <todo/>
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

		/// <todo/>
		public static IBehaviorSetupResult Setup<T>(this Mock<T> mock, Expression<Action<T>> action, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(action, nameof(action));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)action, behaviors);
		}

		/// <todo/>
		public static IBehaviorSetupResult Setup<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <todo/>
		public static IBehaviorSetupResult SetupAdd<T>(this Mock<T> mock, Action<T> addExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(addExpression, nameof(addExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, mock.ConstructorArguments);
			Guard.IsEventAdd(expression, nameof(expression));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <todo/>
		public static IBehaviorSetupResult SetupGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> getterExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(getterExpression, nameof(getterExpression));
			Guard.IsPropertyOrIndexer(getterExpression, nameof(getterExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			return mock.Setup((LambdaExpression)getterExpression, behaviors);
		}

		/// <todo/>
		public static IBehaviorSetupResult SetupRemove<T>(this Mock<T> mock, Action<T> removeExpression, IEnumerable<Behavior> behaviors)
			where T : class
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));
			Guard.NotNull(behaviors, nameof(behaviors));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, mock.ConstructorArguments);
			Guard.IsEventRemove(expression, nameof(expression));

			return mock.Setup((LambdaExpression)expression, behaviors);
		}

		/// <todo/>
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
