using System;

namespace Moq
{
	public interface ICall
	{
		void Throws(Exception exception);
		ICall Callback(Action callback);
	}

	public interface ICall<TResult> : ICall
	{
		void Returns(TResult value);
		void Returns(Func<TResult> valueExpression);
		new ICall<TResult> Callback(Action callback);
	}
}
