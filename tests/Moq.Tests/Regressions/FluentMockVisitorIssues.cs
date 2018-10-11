using Xunit;

namespace Moq.Tests.Regressions
{
	public class FluentMockVisitorIssues
	{
		[Fact]
		public void WhenQueryingWithTwoIndexes_ThenSetsThemDirectly1()
		{
			var foo = Mock.Of<IFoo>(x => x[0].Value == "hello" && x[1].Value == "goodbye");

			Assert.Equal("hello", foo[0].Value); // Fails here as foo[0] is the same object as foo[1] and foo[1].Value == "goodbye", Fixed!
			Assert.Equal("goodbye", foo[1].Value);
		}

		[Fact]
		public void WhenQueryingWithTwoIndexes_ThenSetsThemDirectly2()
		{
			var foo = Mock.Of<IFoo>(x => x[0] == Mock.Of<IBar>(b => b.Value == "hello") && x[1] == Mock.Of<IBar>(b => b.Value == "goodbye"));

			Assert.Equal("hello", foo[0].Value); // These pass no problem
			Assert.Equal("goodbye", foo[1].Value);
		}

		public interface IFoo
		{
			IBar this[int index] { get; }
		}

		public interface IBar
		{
			string Value { get; }
		}
	}
}