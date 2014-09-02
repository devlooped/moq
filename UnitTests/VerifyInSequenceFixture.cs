using	System;
using	Moq.Sequencing;
using	Xunit;

namespace	Moq.Tests
{
	
	public interface RoleWithSingleSimplestMethod
	{
		void Do();
	}

	public interface RoleWithArgumentAndReturnValue
	{
		string Do(int	a);
	}

	public interface RoleForRecursiveMocking
	{
		RoleWithSingleSimplestMethod Do();
	}

	public interface RoleWithProperty
	{
		string Anything	{	get; set;	}
		string AnythingElse	{	get; set;	}
	}

	public class VerifyInSequenceFixture
	{
		[Fact]
		public void	ShouldThrowExceptionWhenUsingStaticVerifyShortcutOnMocksWithDifferentSequences()
		{
			//GIVEN
			var	m1 = new Mock<RoleWithSingleSimplestMethod>()	{	CallSequence = new CallSequence(MockBehavior.Loose)	};
			var	m2 = new Mock<RoleWithSingleSimplestMethod>()	{	CallSequence = new CallSequence(MockBehavior.Loose)	};

			//WHEN
			m1.Object.Do();
			m2.Object.Do();

			//THEN

			Assert.Throws<MockException>(()	=> 
				CallSequence.Verify(
					m1.CallTo(m	=> m.Do()),
					m2.CallTo(m	=> m.Do()))
			);
		}
	}
}

