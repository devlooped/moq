using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class MockSequenceFixture
	{
		[Fact]
		public void RightSequenceSuccess()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);
			
			MockSequence t = new MockSequence();
			a.InSequence(t).Setup(x => x.M(100)).Returns(101);
			b.InSequence(t).Setup(x => x.M(200)).Returns(201);

			a.Object.M(100);
			b.Object.M(200);
		}

		[Fact]
		public void InvalidSequenceFail()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			MockSequence t = new MockSequence();
			a.InSequence(t).Setup(x => x.M(100)).Returns(101);
			b.InSequence(t).Setup(x => x.M(200)).Returns(201);

			Assert.Throws<MockException>(() =>
			{
				b.Object.M(200);
			});
		}

		[Fact]
		public void NoCyclicSequenceFail()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			MockSequence t = new MockSequence();
			a.InSequence(t).Setup(x => x.M(100)).Returns(101);
			b.InSequence(t).Setup(x => x.M(200)).Returns(201);

			Assert.Equal(101, a.Object.M(100));
			Assert.Equal(201, b.Object.M(200));

			Assert.Throws<MockException>(() =>
			{
				Assert.Equal(101, a.Object.M(100));
			});
			Assert.Throws<MockException>(() =>
			{
				Assert.Equal(201, b.Object.M(200));
			});
		}

		[Fact]
		public void CyclicSequenceSuccesss()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			MockSequence t = new MockSequence() { Cyclic = true };
			a.InSequence(t).Setup(x => x.M(100)).Returns(101);
			b.InSequence(t).Setup(x => x.M(200)).Returns(201);

			Assert.Equal(101, a.Object.M(100));
			Assert.Equal(201, b.Object.M(200));
			
			Assert.Equal(101, a.Object.M(100));
			Assert.Equal(201, b.Object.M(200));

			Assert.Equal(101, a.Object.M(100));
			Assert.Equal(201, b.Object.M(200));
		}

		public interface IFoo
		{
			int M(int p);
		}
	}
}
