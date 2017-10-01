using System;
using System.Linq.Expressions;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal sealed class SetupSequentialActionContext<TMock> : ISetupSequentialAction
		where TMock : class
	{
		private int currentStep;
		private int expectationsCount;
		private Mock<TMock> mock;
		private Expression<Action<TMock>> expression;
		private readonly Action callbackAction;

		public SetupSequentialActionContext(
			Mock<TMock> mock,
			Expression<Action<TMock>> expression)
		{
			this.mock = mock;
			this.expression = expression;
			this.callbackAction = () => currentStep++;
		}

		public ISetupSequentialAction Pass()
		{
			var setup = this.GetSetup();
			setup.Callback(DoNothing);
			this.EndSetup(setup);
			return this;
		}

		private void DoNothing() { }

		public ISetupSequentialAction Throws<TException>() where TException : Exception, new()
		{
			var setup = this.GetSetup();
			setup.Throws<TException>();
			this.EndSetup(setup);
			return this;
		}

		public ISetupSequentialAction Throws(Exception exception)
		{
			var setup = this.GetSetup();
			setup.Throws(exception);
			this.EndSetup(setup);
			return this;
		}

		private void EndSetup(ICallback callback)
		{
			callback.Callback(callbackAction);
		}

		private ISetup<TMock> GetSetup()
		{
			var expectationStep = this.expectationsCount;
			this.expectationsCount++;

			return this.mock
				.When(() => currentStep == expectationStep)
				.Setup(expression);
		}
	}
}
