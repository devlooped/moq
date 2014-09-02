using	Moq.Sequencing;
using	Moq.Sequencing.NavigationStrategies;
using	Xunit;

namespace	Moq.Tests
{
	public class VerifyInSequenceFixtureWithStrictCallSequenceStartingAnywhere
	{
		[Fact]
		public void	ShouldPassVerificationOfCallsMadeExactlyInTheVerifiedSequence()
		{
			var	strictLaterSequence	=	new	StrictSequenceStartingAnywhere();
			var	a	=	new	Mock<IFoo>() { CallSequence	=	strictLaterSequence	};
			var	b	=	new	Mock<IFoo>() { CallSequence	=	a.CallSequence };

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
			var	strictLaterSequence	=	new	StrictSequenceStartingAnywhere();

			var	a	=	new	Mock<IFoo> { CallSequence	=	strictLaterSequence	};
			var	b	=	new	Mock<IFoo> { CallSequence	=	a.CallSequence };

			b.Object.Do(200);

			Assert.Throws<MockException>(()	=>
																	 CallSequence.Verify(
																		 a.CallTo(x	=> x.Do(100))
																		 )
				);
		}

		[Fact]
		public void	ShouldNotSupportCyclicSequencing()
		{
			var	sequence = new StrictSequenceStartingAnywhere();
			var	a	=	new	Mock<IFoo>() { CallSequence	=	sequence };
			var	b	=	new	Mock<IFoo>() { CallSequence	=	a.CallSequence };

			a.Object.Do(200);
			b.Object.Do(100);

			Assert.Throws<MockException>(()	=>
																	 CallSequence.Verify(a.CallTo(m	=> m.Do(100)))
				);
		}

		[Fact]
		public void	ShouldMatchCallsInTheRightSequenceButMadeLater()
		{
			var	sequence = new StrictSequenceStartingAnywhere();
			var	a	=	new	Mock<IFoo>() { CallSequence	=	sequence };
			var	b	=	new	Mock<IFoo>() { CallSequence	=	sequence };
			var	c	=	new	Mock<IFoo>() { CallSequence	=	sequence };
			var	d	=	new	Mock<IFoo>() { CallSequence	=	sequence };

			a.Object.Do(2);
			b.Object.Do(1);
			c.Object.Do(2);
			d.Object.Do(3);

			CallSequence.Verify(
				b.CallTo(m =>	m.Do(1)),
				c.CallTo(m =>	m.Do(2))
				);
		}
		
		[Fact]
		public void	ShouldThrowWhenMatchedSequenceIsInterruptedByOtherCalls()
		{
			var	sequence = new StrictSequenceStartingAnywhere();
			var	a	=	new	Mock<IFoo> { CallSequence	=	sequence };
			var	b	=	new	Mock<IFoo> { CallSequence	=	sequence };
			var	c	=	new	Mock<IFoo> { CallSequence	=	sequence };
			var	d	=	new	Mock<IFoo> { CallSequence	=	sequence };

			a.Object.Do(2);
			b.Object.Do(1);
			d.Object.Do(3);
			c.Object.Do(2);
			
			Assert.Throws<MockException>(
				()	=> 
					CallSequence.Verify(
						b.CallTo(m	=> m.Do(1)),
						c.CallTo(m	=> m.Do(2))
					)
				);
		}
		
		public interface IFoo
		{
			int	Do(int arg);
		}
		
	}
}