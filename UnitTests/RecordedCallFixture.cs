using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language;
using Xunit;

namespace Moq.Tests
{
    public class RecordedCallFixture
    {
        public interface ITestInterface
        {
            String StringProperty { get; set; }

            void StringAction(string str);
        }

        [Fact]
        public void ReplayEnumerable_InvokesOnce_AssertsOnce()
        {
            var mock = new Mock<ITestInterface>();
            
            mock.Object.StringAction("test");

            bool didAssert = false;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                            {
                                Assert.Equal("test", str);
                                didAssert = true;
                            });

            Assert.True(didAssert);
        }

        [Fact]
        public void ReplayEnumerable_InvokedTwiceReplayOnce_AssertsOnlyOnce()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("test");

            int assertCount = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertCount++;
                }, Times.Once());

            Assert.Equal(1, assertCount);
        }



        [Fact]
        public void ReplayEnumerable_NoTimesSpecified_ExpectsAtLeastOneCall()
        {
            var mock = new Mock<ITestInterface>();

            var calls = mock.Verify(x => x.StringAction(It.IsAny<String>()), Times.AtMostOnce());

            Assert.Equal(0, calls.Count);
            Assert.Throws<ArgumentException>(() =>
                {
                    calls.Replay((string s) =>
                        {
                            throw new Exception("Should not get here");
                        });
                });
        }

        [Fact]
        public void ReplayEnumerable_TimesUpTo1_DoesntMindNoCalls()
        {
            var mock = new Mock<ITestInterface>();

            var calls = mock.Verify(x => x.StringAction(It.IsAny<String>()), Times.AtMostOnce());

            Assert.Equal(0, calls.Count());

            Assert.DoesNotThrow(() =>
            {
                calls.Replay((string s) =>
                    {

                    }, Times.AtMost(1));
            });
            
        }

        [Fact]
        public void ReplayEnumerable_Invoked2ReplayExactly2_AssertsBoth()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("test");

            int assertCount = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertCount++;
                }, Times.Exactly(2));

            Assert.Equal(2, assertCount);
        }

        [Fact]
        public void ReplayEnumerable_ReplayExactly2_DoesntCatchAssertionFailure()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            
            var calls = mock.Verify(x => x.StringAction(It.IsAny<string>()));

            //assert allows unit test framework assertions through to unit test runner
            Assert.Throws<MockAssertionFailedException>(() =>
            {
                calls.Replay((string str) =>
                    {
                        throw new MockAssertionFailedException("Mock Assert.AreEqual failed");
                    }, Times.Exactly(2));
            });
        }

        [Fact]
        public void ReplayEnumerable_NoTimesParam_ReplaysRemainder()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("efgh");


            int assertCount = 0;
            List<string> given = new List<string>();
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    given.Add(str);
                    assertCount++;
                });

            Assert.Equal(3, assertCount);
            Assert.Equal("test", given[0]);
            Assert.Equal("abcd", given[1]);
            Assert.Equal("efgh", given[2]);
        }

        [Fact]
        public void ReplayEnumerable_NoTimesParam_DoesntCatchAssertFailure()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("efgh");

            Assert.Throws<MockAssertionFailedException>(() =>
                mock.Verify(x => x.StringAction(It.IsAny<string>()))
                    .Replay((string str) =>
                    {
                        throw new MockAssertionFailedException("Mock Assert.AreEqual failed");
                    })
            );
        }

        [Fact]
        public void ReplayEnumerable_ChainedReplaysSpecifyingTimes_ReplaysCorrectParameters()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("abcd");

            int assertFirst = 0;
            int assertSecond = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                            {
                                Assert.Equal("test", str);
                                assertFirst++;
                            }, Times.Once())
                .Replay((string str) =>
                            {
                                Assert.Equal("abcd", str);
                                assertSecond++;
                            });
            Assert.Equal(1, assertFirst);
            Assert.Equal(2, assertSecond);
        }

        [Fact]
        public void ReplayEnumerable_FirstReplayAtMost2_ConsumesRecordedCallsAndMovesOn()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");

            int assertFirst = 0;
            int assertSecond = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertFirst++;
                }, Times.AtMost(2))
                .Replay((string str) =>
                {
                    Assert.Equal("abcd", str);
                    assertSecond++;
                });
            Assert.Equal(2, assertFirst);
            Assert.Equal(1, assertSecond);
        }

        [Fact]
        public void ReplayEnumerable_AssertFailsBefore2_IgnoresAssertionFailureAndMovesOn()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("abcd");

            int assertFirst = 0;
            int assertSecond = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertFirst++;
                }, Times.AtMost(2))
                .Replay((string str) =>
                {
                    Assert.Equal("abcd", str);
                    assertSecond++;
                });
            Assert.Equal(1, assertFirst);
            Assert.Equal(2, assertSecond);
        }

        [Fact]
        public void ReplayEnumerable_FirstReplayBetween_ConsumesRecordedCallsAndMovesOn()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");

            int assertFirst = 0;
            int assertSecond = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertFirst++;
                }, Times.Between(1,2, Range.Inclusive))
                .Replay((string str) =>
                {
                    Assert.Equal("abcd", str);
                    assertSecond++;
                });
            Assert.Equal(2, assertFirst);
            Assert.Equal(1, assertSecond);
        }

        [Fact]
        public void ReplayEnumerable_AssertFailsInBetween_IgnoresAssertionFailureAndMovesOn()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("abcd");

            int assertFirst = 0;
            int assertSecond = 0;
            mock.Verify(x => x.StringAction(It.IsAny<string>()))
                .Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertFirst++;
                }, Times.Between(1, 2, Range.Inclusive))
                .Replay((string str) =>
                {
                    Assert.Equal("abcd", str);
                    assertSecond++;
                });
            Assert.Equal(1, assertFirst);
            Assert.Equal(2, assertSecond);
        }


        [Fact]
        public void ReplayEnumerable_AtLeast2FailsBeforeDone_Fails()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");

            Assert.Throws<MockAssertionFailedException>(() =>
            {
                mock.Verify(x => x.StringAction(It.IsAny<string>()))
                    .Replay((string str) =>
                    {
                        throw new MockAssertionFailedException("Mock Assert.AreEqual failed");
                    }, Times.AtLeast(2));
            });
        }

        [Fact]
        public void ReplayEnumerable_AtLeast3ButOnly2Calls_Fails()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("test");

            int assertCount = 0;
            var calls = mock.Verify(x => x.StringAction(It.IsAny<string>()));

            Assert.Throws<ArgumentException>(() =>
            {
                calls.Replay((string str) =>
                {
                    Assert.Equal("test", str);
                    assertCount++;
                }, Times.AtLeast(3));
            });

            Assert.Equal(2, assertCount);
        }

        [Fact]
        public void Verify_ForEachCall_ReplaysEachInTurn()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("efgh");

            List<string> replayed = new List<string>();
            var calls = mock.Verify(x => x.StringAction(It.IsAny<String>()));
            foreach(var call in calls)
            {
                call.Replay((string s) => replayed.Add(s));
            }

            Assert.Equal(3, replayed.Count);
            Assert.Equal("test", replayed[0]);
            Assert.Equal("abcd", replayed[1]);
            Assert.Equal("efgh", replayed[2]);
        }

        [Fact]
        public void IRecordedCall_Arguments_CorrectArgs()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("efgh");

            var calls = mock.Verify(x => x.StringAction(It.IsAny<String>()));
            
            Assert.Equal("test", calls[0].Arguments[0]);
            Assert.Equal("abcd", calls[1].Arguments[0]);
            Assert.Equal("efgh", calls[2].Arguments[0]);
            Assert.Equal(1, calls[0].Arguments.Count);
            Assert.Equal(1, calls[1].Arguments.Count);
            Assert.Equal(1, calls[2].Arguments.Count);
        }

        [Fact]
        public void IRecordedCall_MethodInfo_CorrectMethodInfo()
        {
            var mock = new Mock<ITestInterface>();

            mock.Object.StringAction("test");
            mock.Object.StringAction("abcd");
            mock.Object.StringAction("efgh");

            var calls = mock.Verify(x => x.StringAction(It.IsAny<String>()));

            var expected = typeof(ITestInterface).GetMethod("StringAction");
            Assert.Equal(expected, calls[0].Method);
            Assert.Equal(expected, calls[1].Method);
            Assert.Equal(expected, calls[2].Method);
        }

        [Fact]
        public void VerifyGet_AllPropertiesSetup_ReturnsAllRecordedCalls()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            string str;
            
            str = mock.Object.StringProperty;
            mock.Object.StringProperty = "test";
            str = mock.Object.StringProperty;

            var calls = mock.VerifyGet(x => x.StringProperty, Times.Exactly(2));

            Assert.Equal(2, calls.Count);
            Assert.Null(calls[0].ReturnValue);
            Assert.Equal("test", calls[1].ReturnValue);
        }

        [Fact]
        public void VerifySet_AllPropertiesSetup_CanReplayReturnedEnumerable()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringProperty = "test";
            mock.Object.StringProperty = null;


            var calls = mock.VerifySet(x => x.StringProperty = It.IsAny<string>(), Times.Exactly(2));

            Assert.Equal(2, calls.Count);
            var callsAfterReplay = calls.Replay((string s) => Assert.Equal("test", s), Times.Once())
                    .Replay((string s) => Assert.Equal(null, s), Times.Once());

            Assert.Equal(0, callsAfterReplay.Count());
        }
        
        [Fact]
        public void ReplayEnumerable_CallbackParameterCountMismatch_ThrowsException()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringAction("test");

            Assert.Throws<ArgumentException>(() =>
            {
                mock.Verify(x => x.StringAction(It.IsAny<string>()))
                    .Replay((string i, object o) =>
                    {
                        throw new Exception("Should not execute this");
                    });
            });
        }
        
        [Fact]
        public void ReplayEnumerable_CallbackParameterMismatch_ThrowsException()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringAction("test");

            var calls = mock.Verify(x => x.StringAction(It.IsAny<string>()));
            Assert.Throws<ArgumentException>(() =>
            {
                calls.Replay((int o) =>
                    {
                        throw new Exception("Should not execute this");
                    });
            });
        }

        [Fact]
        public void Replay_CallbackParameterCountMismatch_ThrowsException()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringAction("test");

            var call = mock.Verify(x => x.StringAction(It.IsAny<string>())).First();
            Assert.Throws<ArgumentException>(() =>
            {
                    call.Replay((int i, object o) =>
                    {
                        throw new Exception("Should not execute this");
                    });
            });
        }
        
        [Fact]
        public void Replay_CallbackParameterMismatch_ThrowsException()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringAction("test");

            var call = mock.Verify(x => x.StringAction(It.IsAny<string>())).First();
            Assert.Throws<ArgumentException>(() =>
            {
                call.Replay((int o) =>
                    {
                        throw new Exception("Should not execute this");
                    });
            });
        }
        
        [Fact]
        public void Replay_CallbackParameterAssignable_Executes()
        {
            var mock = new Mock<ITestInterface>();
            mock.SetupAllProperties();

            mock.Object.StringAction("test");

            var call = mock.Verify(x => x.StringAction(It.IsAny<string>())).First();
            
            call.Replay((object o) =>
                {
                    Assert.True(o is string);
                    Assert.Equal("test", o.ToString());
                });
            
        }

        public interface ITestInterface2
        {
            void FooByRef(ref string str);

            void FooOut(string str1, out string str2);
        }

        [Fact]
        public void VerifyByRef_Replayable()
        {
            var mock = new Mock<ITestInterface2>();

            string s = null;
            mock.Object.FooByRef(ref s);
            s = "test";
            mock.Object.FooByRef(ref s);

            string s2 = null;
            mock.Verify(x => x.FooByRef(ref s2))

                .Replay((string str) =>
                            {
                                Assert.Null(str);
                            }, Times.Once());

            s2 = "test";
            mock.Verify(x => x.FooByRef(ref s2))

                .Replay((string str) =>
                            {
                                Assert.Equal("test", str);
                            });
        }

        [Fact]
        public void VerifyOut_Replayable()
        {
            var mock = new Mock<ITestInterface2>();

            string s;
            mock.Object.FooOut(null, out s);
            s = "testOut";
            mock.Object.FooOut("test", out s);

            string s2;
            mock.Verify(x => x.FooOut(It.IsAny<string>(), out s2), Times.Exactly(2))

                .Replay((string str1, string str2) =>
                            {
                                Assert.Null(str1);
                                Assert.Null(str2);
                            }, Times.Once())

                .Replay((string str1, string str2) =>
                            {
                                Assert.Equal("test", str1);
                                Assert.Equal("testOut", str2);
                            });

        }
    }

    /// <summary>
    /// Mocks a unit test assertion exception to keep these separate from XUnit asserts
    /// </summary>
    class MockAssertionFailedException : Exception
    {
        public MockAssertionFailedException(string message) : base(message){}
    }
    
}
