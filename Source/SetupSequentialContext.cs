using System;
using System.Linq.Expressions;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal class SetupSequentialContext<TMock, TResult> : ISetupSequentialResult<TResult>
		where TMock : class
	{
		private int currentStep;
		private int countOfExpectations;
		private Mock<TMock> mock;
		private Expression<Func<TMock, TResult>> expression;

		public SetupSequentialContext(
			Mock<TMock> mock,
			Expression<Func<TMock, TResult>> expression)
		{
			currentStep = 0;
			countOfExpectations = 0;
			this.mock = mock;
			this.expression = expression;
		}

		private ISetup<TMock, TResult> GetSetup()
		{
			var expectationStep = countOfExpectations;
			countOfExpectations++;

			return this.mock
				.When(() => currentStep == expectationStep)
				.Setup<TResult>(expression);
		}

		private void EndSetup(ICallback callback)
		{
			callback.Callback(() => currentStep++);
		}

		public ISetupSequentialResult<TResult> Returns(TResult value)
		{
			EndSetup(GetSetup().Returns(value));
			return this;
		}

		public void Throws(Exception exception)
		{
			GetSetup().Throws(exception);
		}

		public void Throws<TException>() where TException : Exception, new()
		{
			GetSetup().Throws<TException>();
		}
	}
}