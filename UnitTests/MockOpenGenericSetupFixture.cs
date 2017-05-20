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

		[Fact]
		public void SetupReturns_OpenGenericArgumentWithWhereClause()
		{
			// Arrange
			var mock = new Mock<IHasOpenGeneric>();

			int callCount = 0;
			IPublicCallContext callContext = null;

			mock.Setup(x => x.GetWhere<ItAnySomeBaseClass, ItAnySomeBaseClass, ItAnySomeBaseClass>(It.IsAny<ItAnySomeBaseClass>()))
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
			Assert.Equal(input, (SomeImplementationClass)callContext.Arguments[0]);
			Assert.Equal(typeof(SomeImplementationClass), callContext.Method.ReturnType);
		}

		public class SomeImplementationClass : SomeBaseClass
		{
		}
		public class SomeBaseClass
		{
		}

		public class ItAnySomeBaseClass : SomeBaseClass, It.AnyType
		{
			// Have 2 separate Interfaces, one that is empty and is generic arg definition and the other for parameter matching?
			public object Object { get; set;  }
		}

		public interface IHasOpenGeneric
		{
			TReturnArg Get<TInputArg1, TArgInput2, TReturnArg>(TInputArg1 arg1);

			TReturnArg GetWhere<TInputArg1, TArgInput2, TReturnArg>(TInputArg1 arg1)
				where TInputArg1 : SomeBaseClass
				where TArgInput2 : SomeBaseClass
				where TReturnArg : SomeBaseClass;
		}
	}
}
