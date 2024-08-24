// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
    public class VerifiableSetupFixture
    {
        [Fact]
        public void Verifiable_Times_Never_will_fail_fast_when_setup_invoked_too_often()
        {
            VerifyFailsFastWhenUpperBoundExceeded(Times.Never());
        }

        [Fact]
        public void Verifiable_Times_Once_will_fail_fast_when_setup_invoked_too_often()
        {
            VerifyFailsFastWhenUpperBoundExceeded(Times.Once());
        }

        [Fact]
        public void Verifiable_Times_AtMostOnce_will_fail_fast_when_setup_invoked_too_often()
        {
            VerifyFailsFastWhenUpperBoundExceeded(Times.AtMostOnce());
        }

        [Fact]
        public void Verifiable_Times_AtMost_N_will_fail_fast_when_setup_invoked_too_often()
        {
            VerifyFailsFastWhenUpperBoundExceeded(Times.AtMost(2));
        }

        void VerifyFailsFastWhenUpperBoundExceeded(Times times)
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(times);

            var (_, upperBound) = times;
            for (var i = 0; i < upperBound; ++i)
            {
                mock.Object.Method();
            }
            Action oneInvocationTooMany = () => mock.Object.Method();

            Assert.Throws<MockException>(oneInvocationTooMany);
        }

        [Fact]
        public void Verifiable_Times_AtLeastOnce_will_fail_Verify_when_never_called()
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.AtLeastOnce);

            Assert.Throws<MockException>(mock.Verify);
        }

        [Fact]
        public void Verifiable_Times_AtLeastOnce_will_pass_Verify_when_called()
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.AtLeastOnce);

            mock.Object.Method();

            mock.Verify();
        }

        [Fact]
        public void Verifiable_Times_AtLeast_N_will_fail_Verify_when_not_called_enough_times()
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.AtLeast(2));

            mock.Object.Method();

            Assert.Throws<MockException>(mock.Verify);
        }

        [Fact]
        public void Verifiable_Times_AtLeast_N_will_pass_Verify_when_called_enough_times()
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.AtLeast(2));

            mock.Object.Method();
            mock.Object.Method();

            mock.Verify();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void Verifiable_Times_Exactly_N_will_pass_Verify_when_called_N_times(int n)
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.Exactly(n));
            for (var i = 0; i < n; ++i)
            {
                mock.Object.Method();
            }
            mock.Verify();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void Verifiable_Times_Exactly_N_will_fail_Verify_when_not_called_N_times(int n)
        {
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(Times.Exactly(n));

            for (var i = 0; i < n - 1; ++i)
            {
                mock.Object.Method();
            }

            Assert.Throws<MockException>(mock.Verify);
        }

        [Fact]
        public void Verifiable_Times_can_be_used_to_exclude_a_setup_from_VerifyAll()
        {
            var anyNumberOfTimes = Times.AtMost(int.MaxValue);
            var mock = new Mock<IX>();
            mock.Setup(m => m.Method()).Verifiable(anyNumberOfTimes);
            mock.VerifyAll();
        }

        [Fact]
        public void Verifiable_Times_can_be_used_to_verify_invocation_count_of_reused_mutable_arguments()
        {
            // First, let's introduce the problem: mutating a reused, reference-typed argument affects the recorded invocations,
            // meaning `mock.Verify(call, times)` will only see the final object state instead of the state as it was during the invocations:
            var mock1 = new Mock<IX>();
            var mutableArg1 = new MutableArg { Value = 1 };
            mock1.Object.Method(mutableArg1);
            mock1.Object.Method(mutableArg1);
            mutableArg1.Value = "one";
            Assert.Throws<MockException>(() => mock1.Verify(m => m.Method(It.Is<MutableArg>(arg => arg.Value is int)), Times.Exactly(2)));

            // This can be worked around by explicitly setting up the call and specifying the expected number of calls upfront:
            var mock2 = new Mock<IX>();
            mock2.Setup(m => m.Method(It.Is<MutableArg>(arg => arg.Value is int))).Verifiable(Times.Exactly(2));
            var mutableArg2 = new MutableArg { Value = 1 };
            mock2.Object.Method(mutableArg2);
            mock2.Object.Method(mutableArg2);
            mutableArg2.Value = "one";
            mock2.Verify();
        }

        public interface IX
        {
            void Method();
            void Method(MutableArg arg);
        }

        public class MutableArg
        {
            public object Value { get; set; }
        }
    }
}
