using Xunit;

namespace Moq.Tests
{
	public class MockOpenGenericSetupFixture
	{
		public class SomeImplementationClass : SomeBaseClass
		{
		}

		public class SomeBaseClass
		{
		}

		public interface IHasOpenGeneric
		{
			TReturnArg Get<TInputArg1, TArgInput2, TReturnArg>(TInputArg1 arg1);

			TReturnArg GetWhere<TInputArg1, TArgInput2, TReturnArg>(TInputArg1 arg1)
				where TInputArg1 : SomeBaseClass
				where TArgInput2 : SomeBaseClass
				where TReturnArg : SomeBaseClass;
		}

		[Fact]
		public void SetupReturns_OpenGenericArgumentWithWhereClause()
		{
			// Arrange
			var mock = new Mock<IHasOpenGeneric>();

			var callCount = 0;
			ICallContext callContext = null;

			mock.SetupGeneric(x => x
					.GetWhere<SomeBaseClass, SomeBaseClass, SomeBaseClass>(It.IsAnySubTypeOf<SomeBaseClass>())
				)
				.Returns(context =>
				{
					callContext = context;
					callCount++;
					return new SomeImplementationClass();
				});

			Assert.Equal(0, callCount);

			var input = new SomeImplementationClass();

			// Act

			var res = mock.Object.GetWhere<SomeImplementationClass, SomeImplementationClass, SomeImplementationClass>(input);

			// Assert
			Assert.Equal(1, callCount);

			Assert.IsType<SomeImplementationClass>(res);

			Assert.NotNull(callContext);
			Assert.Equal(1, callContext.Arguments.Length);

			Assert.NotNull(callContext.Arguments[0]);
			Assert.Equal(typeof(SomeImplementationClass), callContext.Arguments[0].GetType());
			Assert.Equal(input, (SomeImplementationClass) callContext.Arguments[0]);
			Assert.Equal(typeof(SomeImplementationClass), callContext.Method.ReturnType);
		}

		[Fact]
		public void SetupReturns_OpenGenericReturnArgument_And_Param()
		{
			// Arrange
			var mock = new Mock<IHasOpenGeneric>();

			var callCount = 0;
			ICallContext callContext = null;

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
			Assert.Equal(2, (int) callContext.Arguments[0]);
			Assert.Equal(typeof(string), callContext.Method.ReturnType);
		}
	}
}