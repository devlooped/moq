using	Moq.Sequencing;
using	Xunit;

namespace	Moq.Tests
{
	public class VerifyInSequenceFixtureWithStrictSequence
	{
		[Fact]
		public void	ShouldPassVerificationOfCallsMadeExactlyInTheVerifiedSequence()
		{
			var	strictSequence = new CallSequence(MockBehavior.Strict);
			var	a	=	new	Mock<IFoo>()
			{
				CallSequence = strictSequence
			};
			var	b	=	new	Mock<IFoo>()
			{
				CallSequence = strictSequence
			};

			a.Object.Do(100);
			b.Object.Do(200);

			CallSequence.Verify(
				a.CallTo(x =>	x.Do(100)),
				b.CallTo(x =>	x.Do(200))
			);
		}

		[Fact]
		public void	ShouldThrowMockExceptionWhenExactSequenceIsNotMatched()
		{
			var	strictSequence = new CallSequence(MockBehavior.Strict);

			var	a	=	new	Mock<IFoo>()
			{
				CallSequence = strictSequence
			};

			var	b	=	new	Mock<IFoo>()
			{
				CallSequence = strictSequence
			};

			b.Object.Do(200);

			Assert.Throws<MockException>(()	=>
																	 CallSequence.Verify(
																		 a.CallTo(m	=> m.Do(100))
																		 )
				);
		}

		[Fact]
		public void	ShouldNotSupportCyclicSequencing()
		{
			var	sequence = new CallSequence(MockBehavior.Strict);

			var	a	=	new	Mock<IFoo>() { CallSequence	=	sequence };
			var	b	=	new	Mock<IFoo>() { CallSequence	=	sequence };

			a.Object.Do(200);
			b.Object.Do(100);

			Assert.Throws<MockException>(()	=>
																	 CallSequence.Verify(
																		 a.CallTo(m	=> m.Do(100)),
																		 b.CallTo(m	=> m.Do(200))
																		 )
				);
		}

		public interface IFoo
		{
			int	Do(int arg);
		}

	}
}