using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq.Proxy;
using Xunit;

namespace Moq.Tests
{
	public class MockOpenGenericSetupFixture
	{
		[Fact]
		public void SetupReturns_OpenGenericReturnArgument_And_Param()
		{
			// Arrange
			var mock = new Mock<IHasOpenGeneric>();

			int callCount = 0;
			IPublicCallContext callContext = null;

			mock.Setup(x => x.Get<It.AnyType, It.AnyType, It.AnyType>(It.IsAny<It.AnyType>()))
				.Returns(context =>
				{
					callContext = context;
					callCount++;
					return "Test";
				});

			Assert.Equal(0, callCount);

			// Act
			var res = mock.Object.Get<int, decimal, string>(2);
			
			// Assert
			Assert.Equal(1, callCount);

			Assert.Equal(res, "Test");

			Assert.NotNull(callContext);
			Assert.Equal(1, callContext.Arguments.Length);

			Assert.NotNull(callContext.Arguments[0]);
			Assert.Equal(typeof(int), callContext.Arguments[0].GetType());
			Assert.Equal(2, (int)callContext.Arguments[0]);
			Assert.Equal(typeof(string), callContext.Method.ReturnType);
		}

		public interface IHasOpenGeneric
		{
			TReturnArg Get<TInputArg1, TArgInput2, TReturnArg>(TInputArg1 arg1);
		}
	}
}
