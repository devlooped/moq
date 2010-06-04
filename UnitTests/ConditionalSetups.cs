using System;
using Xunit;

namespace Moq.Tests
{
	public class ConditionalSetups
	{
		[Fact]
		public void Lang()
		{
			var m = new Mock<IFoo>();

			m.When(() => true)
				.Setup(x => x.M1())
				.Returns("bar");

			m.When(() => true)
				.Setup(x => x.M2());
		}

		[Fact]
		public void ChooseExpectationThatHasAffirmativeCondition()
		{
			var m = new Mock<IFoo>();

			bool first = true;

			m.When(() => first).Setup(x => x.M1()).Returns("bar");
			m.When(() => !first).Setup(x => x.M1()).Returns("no bar");

			Assert.Equal("bar", m.Object.M1());
			first = false;
			Assert.Equal("no bar", m.Object.M1());
			first = true;
			Assert.Equal("bar", m.Object.M1());
		}

		public interface IFoo
		{
			string M1();
			void M2();
		}
	}
}
