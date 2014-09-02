using	System;
using	System.Linq.Expressions;
using	Moq.Sequencing.Extensibility;

namespace	Moq.Sequencing
{
	internal class CallVerificationStep<T> : IVerificationStep where T : class
	{
		readonly Mock<T> _mock;
		readonly Expression<Action<T>> _action;

		public CallVerificationStep(Mock<T>	mock,	Expression<Action<T>>	action)
		{
			_mock	=	mock;
			_action	=	action;
		}

		public void	Verify()
		{
			_mock.VerifyInSequence(_action);
		}

		public CallSequence	CallSequence { get { return	_mock.CallSequence;	}	}
	}
}