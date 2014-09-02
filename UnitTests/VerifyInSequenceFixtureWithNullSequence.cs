using Moq.Sequencing;
using Xunit;

namespace Moq.Tests
{
  public class VerifyInSequenceFixtureWithNullSequence
  {
    [Fact]
    public void ShouldThrowExceptionWhenVerifyingCallOrderOnMockWithNullSequence()
    {
      var sequence = CallSequence.None();
      var mock1 = new Mock<RoleWithSingleSimplestMethod>()
      {
        CallSequence = sequence
      };

      var mock2 = new Mock<RoleWithSingleSimplestMethod>()
      {
        CallSequence = sequence
      };


      mock1.Object.Do();
      mock2.Object.Do();

      Assert.Throws<NoSequenceAssignedException>(
        () => CallSequence.Verify(mock1.CallTo(m => m.Do())
       ));
    }

    [Fact]
    public void ShouldThrowExceptionWhenOrderVerificationIsPerformedForSettingProperty()
    {
      var something = "something";
      var mock1 = new Mock<RoleWithProperty>();

      mock1.Object.Anything = something;

      Assert.Throws<NoSequenceAssignedException>(() =>
        CallSequence.Verify(mock1.CallToSet(m => m.Anything = something))
      );
    }

    [Fact]
    public void ShouldThrowExceptionWhenOrderVerificationIsPerformedForGettingProperty()
    {
      var mock1 = new Mock<RoleWithProperty>();

      var anything = mock1.Object.Anything;

      Assert.Throws<NoSequenceAssignedException>(
        () => CallSequence.Verify(mock1.CallToGet(m => m.Anything))
      );
    }
  }
}