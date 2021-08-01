// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Moq.Protected;
using System;
using System.Collections.Generic;
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

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			b.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			a.Object.Do(100);
			b.Object.Do(200);
		}

		[Fact]
		public void InvalidSequenceFail()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			b.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Throws<MockException>(() => b.Object.Do(200));
		}

		[Fact]
		public void NoCyclicSequenceFail()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			b.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, b.Object.Do(200));

			Assert.Throws<MockException>(() => a.Object.Do(100));
			Assert.Throws<MockException>(() => b.Object.Do(200));
		}

		[Fact]
		public void CyclicSequenceSuccesss()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var b = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence { Cyclic = true };
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			b.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, b.Object.Do(200));

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, b.Object.Do(200));

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, b.Object.Do(200));
		}

		[Fact]
		public void SameMockRightSequenceConsecutiveInvocationsWithSameArguments()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(102);
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(103);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(102, a.Object.Do(100));
			Assert.Equal(201, a.Object.Do(200));
			Assert.Equal(103, a.Object.Do(100));
		}

		[Fact]
		public void SameMockRightSequenceSuccess()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, a.Object.Do(200));
			Assert.Throws<MockException>(() => a.Object.Do(100));
			Assert.Throws<MockException>(() => a.Object.Do(200));
		}

		[Fact]
		public void SameMockInvalidSequenceFail()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Throws<MockException>(() => a.Object.Do(200));
		}

		[Fact]
		public void MockSequenceBaseShouldThrowIfNoMocksInConstructor()
		{
			var exception = Assert.Throws<ArgumentException>(() => new AMockSequence(false));
			Assert.StartsWith("No mocks", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseShouldThrowIfSetupDoesNotSetup()
		{
			var mock = new Mock<IFoo>();
			var sequence = new AMockSequence(false, mock);
			var exception = Assert.Throws<ArgumentException>(() => sequence.Setup(() => { },_ => 1));
			Assert.StartsWith("No setup performed", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseFindsSetupsFromActions()
		{
			var contextCounter = 0;
			var mock = new Mock<IFoo>();
			var nestedMock = new Mock<IHaveNested>();
			var aMockSequence = new AMockSequence(false,mock, nestedMock);
			ITrackedSetup<int> trackedSetup = null;
			int order = -1;
			Func<ISequenceSetup<int>, int> callback = (sequenceSetup) =>
			 {
				 trackedSetup = sequenceSetup.TrackedSetup;
				 order = sequenceSetup.SetupIndex;
				 return contextCounter++;
			 };
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)), callback);
			Assert.Equal(0, order);
			Assert.Equal("MockSequenceFixture.IFoo f => f.Do(1)", trackedSetup.Setup.ToString());
			var sequenceSetups = trackedSetup.SequenceSetups;
			var sequenceSetup = sequenceSetups[0];
			Assert.Single(sequenceSetups);
			Assert.Equal(contextCounter - 1, sequenceSetup.Context);
			Assert.Equal(order, sequenceSetup.SetupIndex);
			var repeatedSetup = trackedSetup;
			var allTrackedSetups = aMockSequence.GetAllTrackedSetups();
			var trackedSetups = aMockSequence.GetTrackedSetups();
			Assert.Single(allTrackedSetups);
			Assert.Single(trackedSetups);
			Assert.Same(trackedSetup, allTrackedSetups[0]);
			Assert.Same(trackedSetup, trackedSetups[0]);

			aMockSequence.Setup(() =>
			{
				nestedMock.Setup(n => n.ReturnNested(1).DoNested(1));
			}, callback);
			Assert.Equal(1, order);
			Assert.Equal("MockSequenceFixture.INested ... => ....DoNested(1)", trackedSetup.Setup.ToString());
			Assert.Equal(3, allTrackedSetups.Count);
			Assert.Equal(2, trackedSetups.Count);
			Assert.Contains(trackedSetup, allTrackedSetups);
			Assert.Same(trackedSetup, trackedSetups[1]);

			// repeatedSetup
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)), callback);
			Assert.Equal(2, order);
			Assert.Equal(3, allTrackedSetups.Count);
			Assert.Equal(2, trackedSetups.Count);
			Assert.Same(repeatedSetup, trackedSetup);
			sequenceSetups = trackedSetup.SequenceSetups;
			Assert.Equal(2, sequenceSetups.Count);
			Assert.Same(sequenceSetup, sequenceSetups[0]);
			sequenceSetup = sequenceSetups[1];
			Assert.Equal(contextCounter - 1, sequenceSetup.Context);
			Assert.Equal(order, sequenceSetup.SetupIndex);
			
		}

		[Fact]
		public void MockSequenceBaseInvokesConditionWithTrackedSetupAndExecutionIndex()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(false,mock);
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)), (sequenceSetup) => sequenceSetup.SetupIndex);
			mock.Object.Do(1);
			Assert.Single(aMockSequence.ConditionCalls);
			var conditionCall = aMockSequence.ConditionCalls[0];
			Assert.Equal(0,conditionCall.executionOrder);
			var trackedSetup = conditionCall.trackedSetup;
			Assert.Equal("MockSequenceFixture.IFoo f => f.Do(1)", trackedSetup.Setup.ToString());
			var sequenceSetup = trackedSetup.SequenceSetups[0];
			Assert.Equal(0, sequenceSetup.SetupIndex);
			Assert.Equal(0, sequenceSetup.Context);
		}

		[Fact]
		public void MockSequenceBaseShouldCaptureInvocations()
		{
			var mock = new Mock<IFoo>();
			var nestedMock = new Mock<IHaveNested>();
			// listening from non sequence setups works if done before hand.... 
			// but it does not update after....
			nestedMock.Setup(n => n.ReturnNested(1).DoNested(2)).Returns(1);
			var aMockSequence = new AMockSequence(false, mock, nestedMock);

			mock.Object.Do(1);
			Assert.Single(aMockSequence.sequenceInvocations);

			nestedMock.Object.ReturnNested(1).DoNested(1);
			Assert.Equal(3, aMockSequence.sequenceInvocations.Count);

		}

		[Fact]
		public void MockSequenceBaseTracksExecutionIndices()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var aMockSequence = new AMockSequence(false, mock);
			IReadOnlyList<int> executionIndices1 = null;
			aMockSequence.Setup(() => mock.Setup(m => m.Do(1)), (sequenceSetup) =>
			 {
				 executionIndices1 = sequenceSetup.TrackedSetup.ExecutionIndices;
				 return 1;
			 });
			IReadOnlyList<int> executionIndices2 = null;
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)), (sequenceSetup) =>
			{
				executionIndices2 = sequenceSetup.TrackedSetup.ExecutionIndices;
				return 2;
			});
			
			Assert.Empty(executionIndices1);
			Assert.Empty(executionIndices2);

			mocked.Do(3);
			Assert.Empty(executionIndices1);
			Assert.Empty(executionIndices2);

			mocked.Do(1);
			Assert.Single(executionIndices1);
			Assert.Equal(0, executionIndices1[0]);
			Assert.Empty(executionIndices2);

			mocked.Do(2);
			mocked.Do(1);
			Assert.Equal(2, executionIndices1.Count);
			Assert.Equal(2, executionIndices1[1]);
			Assert.Single(executionIndices2);
			Assert.Equal(1, executionIndices2[0]);


		}

		[Fact]
		public void MockSequenceBaseCanCheckInvocationsAgainstSequenceSetups()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(false,mock);
			mock.Setup(m => m.Do(1));
			mock.Object.Do(1);
			Assert.Single(aMockSequence.sequenceInvocations);
			Assert.False(aMockSequence.InvocationsHaveMatchingSetups());

			aMockSequence = new AMockSequence(false, mock);
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)), sequenceSetup => sequenceSetup.SetupIndex);
			mock.Object.Do(2);
			Assert.Single(aMockSequence.sequenceInvocations);
			Assert.True(aMockSequence.InvocationsHaveMatchingSetups());

		}

		[Fact]
		public void MockSequenceBaseStrictInvokesStrictnessFailureForUnmatchedInvocations()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(true, mock);
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)), sequenceSetup => sequenceSetup.SetupIndex);
			
			mock.Object.Do(1);
			Assert.False(aMockSequence.StrictFailure);

			// strictness test occurs when condition returns true
			mock.Object.Do(2);
			Assert.True(aMockSequence.StrictFailure);
		}

		[Fact]
		public void MockSequenceBaseStrictThrowsStrictSequenceExceptionForUnmatchedInvocations()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(true, mock);
			aMockSequence.CallBaseForStrictFailure = true;
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)), sequenceSetup => sequenceSetup.SetupIndex);
			mock.Object.Do(1);

			// strictness test occurs when condition returns true
			Assert.Throws<StrictSequenceException>(() => mock.Object.Do(2));
		}

		private void LooseNewMockSequenceTestBase(Action<Mock<IFoo>,IProtectedAsMock<Protected,ProtectedLike>, NewMockSequence> setupSequence,Action<IFoo,Protected> act)
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();

			var protectedMocked = protectedMock.Object;
			var mockSequence = new NewMockSequence(false, mock, protectedMock);
			setupSequence(mock, protectedAsMock, mockSequence);
			act(mocked, protectedMocked);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfNotCalled()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			 {
				 
			 });
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfCalledInOrder_OneTime()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfNotCalledInOrder_OneTime()
		{
			Assert.Throws<SequenceException>(() => 
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					protectedMocked.InvokeProtectedDo(1);
					mocked.Do(1);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfCalledCorrectExactTimes()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfNotCalledCorrectExactTimes()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					protectedMocked.InvokeProtectedDo(1);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtLeastTimesIsMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.AtLeast(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtLeastTimesIsMetMoreThan()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.AtLeast(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfAtLeastTimesIsNotMet()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.AtLeast(2));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					protectedMocked.InvokeProtectedDo(1);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfNeverCalled()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
					mockSequence.Setup(() => mock.Setup(m => m.Do(2)),Times.Never());
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(2);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtMostMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.AtMost(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfInvokedMoreThanAtMost()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)),Times.AtMost(2));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfBetweenInclusiveMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(0, 2, Range.Inclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfBetweenInclusiveTooManyInvocations()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(0,2,Range.Inclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);
				})
			);
		}

		[Fact]
		public void MockSequenceShouldThrowIfBetweenInclusiveTooFewInvocations()
		{
			Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(1, 2, Range.Inclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					
					protectedMocked.InvokeProtectedDo(1);
				})
			);
		}

		[Fact]
		public void MockSequenceWillNotThrowWhenSkipOptionalTimesSetups()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)), NewMockSequence.OptionalTimes());
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(2)), NewMockSequence.OptionalTimes());
				mockSequence.Setup(() => mock.Setup(m => m.Do(2)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(2);
			});
		}

		[Fact]
		public void MockSequenceWillThrowWhenSkipNonOptionalTimesSetups()
		{
			Assert.Throws<SequenceException>(() =>
			{
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)), NewMockSequence.OptionalTimes());
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(2)), Times.Once());
					mockSequence.Setup(() => mock.Setup(m => m.Do(2)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(2);
				});
			});
		}

		[Fact]
		public void MockSequenceCanUseVerifiableSetupToVerifyASequenceSetup()
		{
			VerifiableSetup verifiableSetup1 = null;
			VerifiableSetup verifiableSetup2 = null;
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				verifiableSetup1 = mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				verifiableSetup2 = mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
			});

			verifiableSetup1.Verify();
			Assert.Throws<SequenceException>(() => verifiableSetup2.Verify(Times.Once()));
		}

		[Fact]
		public void MockSequenceCanUseVerifiableSetupToVerifyAllSameSequenceSetups()
		{
			VerifiableSetup verifiableSetup1 = null;
			VerifiableSetup verifiableSetup2 = null;
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				verifiableSetup1 = mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				verifiableSetup2 = mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});

			verifiableSetup1.VerifyAll(Times.Once());
			Assert.Throws<SequenceException>(() => verifiableSetup2.VerifyAll());
		}

		[Fact]
		public void MockSequenceVerifyVerifiesThatAllSetupsHaveBeenMet()
		{
			NewMockSequence sequence = null;
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				sequence = mockSequence;
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
			});
			Assert.Throws<SequenceException>(() => sequence.Verify());

			NewMockSequence sequence2 = null;
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				sequence2 = mockSequence;
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
			});
			Assert.Throws<SequenceException>(() => sequence2.Verify());

			NewMockSequence sequence3 = null;
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				sequence3 = mockSequence;
			}, (mocked, protectedMocked) =>
			{
				mocked.Do(1);
				mocked.Do(1);
				protectedMocked.InvokeProtectedDo(1);
			});
			sequence3.Verify();
		}

		[Fact]
		public void MockSequenceVerifyStrictVerifiesNoInvocationsWithoutSequenceSetup()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();

			var protectedMocked = protectedMock.Object;
			var mockSequence = new NewMockSequence(true, mock, protectedMock);
			mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
			mocked.Do(1);
			mocked.Do(2);

			Assert.Throws<StrictSequenceException>(mockSequence.Verify);
		}

		[Fact]
		public void MockSequenceVerifyLooseDoesNotVerifyInvocationsWithoutSequenceSetup()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();

			var protectedMocked = protectedMock.Object;
			var mockSequence = new NewMockSequence(false, mock, protectedMock);
			mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
			mocked.Do(1);
			mocked.Do(2);

			mockSequence.Verify();
		}

		[Fact]
		public void MockSequenceShouldThrowWithConsecutiveSameSetups()
		{
			var mock = new Mock<IFoo>();
			var mockSequence = new NewMockSequence(false, mock);
			
			void Setup()
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
			}

			Setup();
			var exception = Assert.Throws<ArgumentException>(Setup);
			Assert.StartsWith("Consecutive setups are the same", exception.Message);
		}

		internal class AMockSequence : MockSequenceBase<int>
		{
			public bool StrictFailure { get; set; }
			public bool CallBaseForStrictFailure { get; set; }
			public AMockSequence(bool strict, params Mock[] mocks) : base(strict, mocks) { }
			public void Setup(Action setup,Func<ISequenceSetup<int>,int> setupCallback)
			{
				base.InterceptSetup(setup,setupCallback);
			}

			public List<(ITrackedSetup<int> trackedSetup, int executionOrder)> ConditionCalls = new List<(ITrackedSetup<int>, int)>();

			protected override bool Condition(ITrackedSetup<int> trackedSetup, int invocationIndex)
			{
				ConditionCalls.Add((trackedSetup, invocationIndex));
				return true;
			}

			internal List<TrackedSetup<int>> GetAllTrackedSetups()
			{
				return allTrackedSetups;
			}

			internal IReadOnlyList<ITrackedSetup<int>> GetTrackedSetups()
			{
				return TrackedSetups;
			}
			internal IReadOnlyList<ISequenceSetup<int>> GetSequenceSetups()
			{
				return SequenceSetups;
			}

			public bool InvocationsHaveMatchingSetups()
			{
				return base.InvocationsHaveMatchingSequenceSetup();
			}

			protected override void StrictnessFailure(IEnumerable<SequenceInvocation> unmatchedInvocations)
			{
				StrictFailure = true;
				if (CallBaseForStrictFailure)
				{
					base.StrictnessFailure(unmatchedInvocations);
				}
			}

			protected override void VerifyImpl()
			{
				throw new NotImplementedException();
			}
		}

		public interface IHaveNested
		{
			INested ReturnNested(int arg);
		}

		public interface INested
		{
			int DoNested(int arg);
		}

		public interface IFoo
		{
			int Do(int arg);
		}
	
		public abstract class Protected
		{
			protected abstract int ProtectedDo(int arg);
			public int InvokeProtectedDo(int arg)
			{
				return ProtectedDo(arg);
			}
		}

		public interface ProtectedLike
		{
			int ProtectedDo(int arg);
		}
	}
}
