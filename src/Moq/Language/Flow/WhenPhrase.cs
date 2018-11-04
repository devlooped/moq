// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq.Language.Flow
{
	internal sealed class WhenPhrase<T> : ISetupConditionResult<T>
		where T : class
	{
		private Mock<T> mock;
		private Condition condition;

		public WhenPhrase(Mock<T> mock, Condition condition)
		{
			this.mock = mock;
			this.condition = condition;
		}

		public ISetup<T> Setup(Expression<Action<T>> expression)
		{
			var setup = Mock.SetupVoid(mock, expression, this.condition);
			return new VoidSetupPhrase<T>(setup);
		}

		public ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
		{
			var setup = Mock.SetupNonVoid(mock, expression, this.condition);
			return new NonVoidSetupPhrase<T, TResult>(setup);
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var setup = Mock.SetupGet(mock, expression, this.condition);
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression)
		{
			var setup = Mock.SetupSet(mock, setterExpression, this.condition);
			return new SetterSetupPhrase<T, TProperty>(setup);
		}

		public ISetup<T> SetupSet(Action<T> setterExpression)
		{
			var setup = Mock.SetupSet(mock, setterExpression, this.condition);
			return new VoidSetupPhrase<T>(setup);
		}
	}
}
