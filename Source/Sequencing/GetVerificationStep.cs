using	System;
using	System.Linq.Expressions;
using	Moq.Sequencing.Extensibility;

namespace	Moq.Sequencing
{
	internal class GetVerificationStep<T,	TProperty> : IVerificationStep where T : class
	{
		readonly Mock<T> _mock;
		readonly Expression<Func<T,	TProperty>>	_action;

		public GetVerificationStep(Mock<T> mock, Expression<Func<T,	TProperty>>	action)
		{
			_mock	=	mock;
			_action	=	action;
		}

		public void	Verify()
		{
			_mock.VerifyGetInSequence(_action);
		}
		
		public CallSequence	CallSequence { get { return	_mock.CallSequence;	}	}
	}
}