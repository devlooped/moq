using Xunit;
namespace Moq.Tests 
{
	public class ArgumentTypeFixture 
	{	
		[Fact]
		public void AnyMatcherShouldOnlyMatchIfTypesAreAssignable() {
			var mock = new Mock<IExecutor>();

			mock.Setup(e => e.Execute(It.IsAny<Test1>())).Returns(1);
			mock.Setup(e => e.Execute(It.IsAny<Test2>())).Returns(2);

			Assert.Equal(1, mock.Object.Execute(new Test1()));
			Assert.Equal(2, mock.Object.Execute(new Test2()));
		}
		
		[Fact]
		public void PredicateMatcherShouldOnlyMatchIfTheTypesAreAssignable() 
		{
			var mock = new Mock<IExecutor>();
			
			mock.Setup(e => e.Execute(It.Is<Test1>(x => x.Property == 1))).Returns(1);
			mock.Setup(e => e.Execute(It.Is<Test2>(x => x.Property == 2))).Returns(2);

			Assert.Equal(1, mock.Object.Execute(new Test1 { Property = 1 }));
			Assert.Equal(2, mock.Object.Execute(new Test2 { Property = 2 }));
		}

		public interface IExecutor 
		{
			int Execute(IInterface something);
		}

		public interface IInterface {}
		
		public class Test1 : IInterface 
		{
			public int Property { get; set; }
		}

		public class Test2 : IInterface 
		{
			public int Property { get; set; }
		}
	}
}
