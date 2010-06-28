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
		private int expectationsCount;
		private Mock<TMock> mock;
		private Expression<Func<TMock, TResult>> expression;

		public SetupSequentialContext(
			Mock<TMock> mock,
			Expression<Func<TMock, TResult>> expression)
		{
			this.mock = mock;
			this.expression = expression;
		}

		private ISetup<TMock, TResult> GetSetup()
		{
			var expectationStep = this.expectationsCount;
			this.expectationsCount++;

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
			this.EndSetup(GetSetup().Returns(value));
			return this;
		}

		public void Throws(Exception exception)
		{
			this.GetSetup().Throws(exception);
		}

		public void Throws<TException>() where TException : Exception, new()
		{
			this.GetSetup().Throws<TException>();
		}
	}
}