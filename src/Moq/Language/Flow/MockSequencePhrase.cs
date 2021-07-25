using System;
using System.Linq.Expressions;

namespace Moq.Language.Flow
{
	internal abstract class MockSequencePhraseBase<T, TAnalog> : ISequenceSetupConditionResult<T, TAnalog> where T : class where TAnalog : class
	{
		protected Mock<T> mock;
		private IMockSequence mockSequence;

		public MockSequencePhraseBase(Mock<T> mock, IMockSequence mockSequence)
		{
			this.mock = mock;
			this.mockSequence = mockSequence;
		}

		private MethodCall DoSetup(Func<Condition, MethodCall> setup)
		{
			var index = mockSequence.SetupIndex;
			var sequenceSetup = setup(new Condition(() => mockSequence.Condition(index), () => mockSequence.SequenceSetupExecuted(index)));
			mockSequence.AddSetup(sequenceSetup, index, mock);
			mockSequence.SetupIndex++;
			return sequenceSetup;
		}
		public ISetup<T> Setup(Expression<Action<TAnalog>> expression)
		{
			var setup = DoSetup(condition => Mock.Setup(mock, GetSetupExpression(expression), condition));
			return new VoidSetupPhrase<T>(setup);
		}

		protected virtual LambdaExpression GetSetupExpression(Expression<Action<TAnalog>> expression)
		{
			return expression;
		}

		public ISetup<T, TResult> Setup<TResult>(Expression<Func<TAnalog, TResult>> expression)
		{
			var setup = DoSetup(condition => Mock.Setup(mock, GetSetupExpression<TResult>(expression), condition));
			return new NonVoidSetupPhrase<T, TResult>(setup);
		}

		protected virtual LambdaExpression GetSetupExpression<TResult>(Expression<Func<TAnalog, TResult>> expression)
		{
			return expression;
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<TAnalog, TProperty>> expression)
		{
			var setup = DoSetup(condition => Mock.SetupGet(mock, GetSetupGetExpression<TProperty>(expression), condition));
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		protected virtual LambdaExpression GetSetupGetExpression<TProperty>(Expression<Func<TAnalog, TProperty>> expression)
		{
			return expression;
		}

		public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<TAnalog> setterExpression)
		{
			return new SetterSetupPhrase<T, TProperty>(SetupSetMethodCall(setterExpression));
		}

		protected abstract LambdaExpression GetSetupSetExpression(Action<TAnalog> setterExpression);
		

		public ISetup<T> SetupSet(Action<TAnalog> setterExpression)
		{
			return new VoidSetupPhrase<T>(SetupSetMethodCall(setterExpression));
		}

		private MethodCall SetupSetMethodCall(Action<TAnalog> setterExpression)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));
			return DoSetup(condition => Mock.SetupSet(mock, GetSetupSetExpression(setterExpression), condition));
		}
	}

	internal sealed class MockSequencePhrase<T> : MockSequencePhraseBase<T, T>
		where T : class
	{
		public MockSequencePhrase(Mock<T> mock, IMockSequence mockSequence) : base(mock, mockSequence) { }
		
		protected override LambdaExpression GetSetupSetExpression(Action<T> setterExpression)
		{
			return ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.mock.ConstructorArguments);
		}
		
	}

}
