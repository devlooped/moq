using	System;
using	Moq.Sequencing.Extensibility;

namespace	Moq.Sequencing
{
	internal class SetVerificationStep<T>	:	IVerificationStep	where	T	:	class
	{
		readonly Mock<T> _mock;
		readonly Action<T> _action;

		public SetVerificationStep(Mock<T> mock, Action<T> action)
		{
			_mock	=	mock;
			_action	=	action;
		}

		public void	Verify()
		{
			_mock.VerifySetInSequence(_action);
		}

		public CallSequence	CallSequence { get { return	_mock.CallSequence;	}	}
	}
}