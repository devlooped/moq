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
		private readonly Action callbackAction;

		public SetupSequentialContext(
			Mock<TMock> mock,
			Expression<Func<TMock, TResult>> expression)
		{
			this.mock = mock;
			this.expression = expression;
			this.callbackAction = () => currentStep++;
		}

		public ISetupSequentialResult<TResult> CallBase()
		{
			this.EndSetup(GetSetup().CallBase());
			return this;
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
			callback.Callback(callbackAction);
		}

		private void EndSetup(ICallback<TMock, TResult> callback)
		{
			callback.Callback(callbackAction);
		}

		public ISetupSequentialResult<TResult> Returns(TResult value)
		{
			this.EndSetup(GetSetup().Returns(value));
			return this;
		}

		public ISetupSequentialResult<TResult> Throws(Exception exception)
		{
            var setup = this.GetSetup();
            setup.Throws(exception);
			this.EndSetup(setup);
			return this;
		}

		public ISetupSequentialResult<TResult> Throws<TException>() where TException : Exception, new()
		{
			var setup = this.GetSetup();
			setup.Throws<TException>();
			this.EndSetup(setup);
			return this;
		}
	}
}