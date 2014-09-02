using Moq.Sequencing;
using Xunit;

namespace Moq.Tests
{
  public class VerifyInSequenceFixtureWithCallSequence
  {
    [Fact]
    public void ShouldNotThrowAnyExceptionWhenCallsAreMadeInOrderInCallSequence()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };

      mock1.Object.Do();
      mock2.Object.Do();

      CallSequence.Verify(
        mock1.CallTo(m => m.Do()),
        mock2.CallTo(m => m.Do())
        );
    }

    [Fact]
    public void ShouldThrowExceptionWhenCallsAreNotMadeInOrderInCallSequence()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };

      mock2.Object.Do();
      mock1.Object.Do();

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallTo(m => m.Do()),
                                     mock2.CallTo(m => m.Do())
                                     )
        );
    }

    [Fact]
    public void ShouldIgnoreCallsInbetweenMatchingCalls()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock3 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };

      mock1.Object.Do();
      mock3.Object.Do();
      mock2.Object.Do();

      CallSequence.Verify(
        mock1.CallTo(m => m.Do()),
        mock2.CallTo(m => m.Do())
        );
    }

    [Fact]
    public void ShouldIgnoreCallsInbetweenUnmatchingCalls()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock3 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };

      mock2.Object.Do();
      mock3.Object.Do();
      mock1.Object.Do();


      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallTo(m => m.Do()),
                                     mock2.CallTo(m => m.Do())
                                     )
        );
    }

    [Fact]
    public void ShouldAllowAMixOfSequencedAndNonSequencedMocks()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithSingleSimplestMethod> { CallSequence = sequence };
      var mock3 = new Mock<RoleWithSingleSimplestMethod>();

      mock1.Object.Do();
      mock3.Object.Do();
      mock2.Object.Do();

      CallSequence.Verify(
        mock1.CallTo(a => a.Do()),
        mock2.CallTo(b => b.Do())
        );
    }

    [Fact]
    public void ShouldProperlyRecognizeNonMatchingArgumentsInSequentialVerification()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithArgumentAndReturnValue> { CallSequence = sequence };
      var mock2 = new Mock<RoleWithArgumentAndReturnValue> { CallSequence = sequence };

      mock2.Object.Do(1);
      mock1.Object.Do(2);
      mock2.Object.Do(3);

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallTo(m => m.Do(2)),
                                     mock2.CallTo(m => m.Do(1))
                                     )
        );

    }

    [Fact]
    public void ShouldRecognizeSequenceOfCallsMadeOnTheSameMock()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleWithArgumentAndReturnValue> { CallSequence = sequence };

      mock1.Object.Do(1);
      mock1.Object.Do(2);
      mock1.Object.Do(3);

      CallSequence.Verify(
        mock1.CallTo(m => m.Do(2)),
        mock1.CallTo(m => m.Do(3))
        );

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallTo(m => m.Do(2)),
                                     mock1.CallTo(m => m.Do(3)),
                                     mock1.CallTo(m => m.Do(1))
                                     )
        );
    }

    [Fact]
    public void ShouldSupportMockChainingWithMatchingCalls()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleForRecursiveMocking>()
      {
        CallSequence = sequence,
        DefaultValue = DefaultValue.Mock
      };

      mock1.Object.Do().Do();

      CallSequence.Verify(
        mock1.CallTo(m => m.Do()),
        mock1.CallTo(m => m.Do().Do())
        );
    }

    [Fact]
    public void ShouldSupportMockChainingWithUnmatchingCalls()
    {
      var sequence = new CallSequence();
      var mock1 = new Mock<RoleForRecursiveMocking>()
      {
        CallSequence = sequence,
        DefaultValue = DefaultValue.Mock
      };

      mock1.Object.Do().Do();

      CallSequence.Verify(
        mock1.CallTo(m => m.Do().Do())
        );

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallTo(m => m.Do().Do()),
                                     mock1.CallTo(m => m.Do())
                                     )
        );
    }

    [Fact]
    public void ShouldAllowVerifyingSettingPropertyInSequence()
    {
      var something = "something";
      var mock1 = new Mock<RoleWithProperty> { CallSequence = new CallSequence() };

      mock1.Object.Anything = something;
      mock1.Object.Anything = something + something;

      CallSequence.Verify(
        mock1.CallToSet(m => m.Anything = something),
        mock1.CallToSet(m => m.Anything = something + something)
        );
    }

    [Fact]
    public void ShouldThrowExceptionWhenVerifiedPropertySetDoNotMatchActual()
    {
      var something = "something";
      var mock1 = new Mock<RoleWithProperty> { CallSequence = new CallSequence() };

      mock1.Object.Anything = something;
      mock1.Object.Anything = something + something;

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallToSet(m => m.Anything = something + something),
                                     mock1.CallToSet(m => m.Anything = something)
                                     )
        );
    }

    [Fact]
    public void ShouldAllowVerifyingGettingPropertyInSequence()
    {
      var sequence = new CallSequence();
      string something;
      var mock1 = new Mock<RoleWithProperty> { CallSequence = sequence };

      something = mock1.Object.Anything;
      something = mock1.Object.AnythingElse;

      CallSequence.Verify(
        mock1.CallToGet(m => m.Anything),
        mock1.CallToGet(m => m.AnythingElse));

      CallSequence.Verify(
        mock1.CallToGet(m => m.Anything),
        mock1.CallToGet(m => m.AnythingElse));
    }

    [Fact]
    public void ShouldThrowExceptionWhenVerifiedPropertyGetDoNotMatchActual()
    {
      string something;
      var mock1 = new Mock<RoleWithProperty> { CallSequence = new CallSequence() };

      something = mock1.Object.Anything;
      something = mock1.Object.AnythingElse;

      Assert.Throws<MockException>(() =>
                                   CallSequence.Verify(
                                     mock1.CallToGet(m => m.AnythingElse),
                                     mock1.CallToGet(m => m.Anything)
                                     )
        );
    }



  }
}