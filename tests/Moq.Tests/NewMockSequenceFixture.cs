// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Moq.Protected;
using System;
using System.Collections.Generic;
using Xunit;

namespace Moq.Tests
{
	public class NewMockSequenceFixture
	{
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
			var exception = Assert.Throws<ArgumentException>(() => sequence.Setup(() => { }));
			Assert.StartsWith("No setup performed", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseFindsSetupsFromActions()
		{
			var mock = new Mock<IFoo>();
			var nestedMock = new Mock<IHaveNested>();
			var aMockSequence = new AMockSequence(false, mock, nestedMock);
			DefaultInvocationShapeSetups invocationShapeSetups = null;
			DefaultSequenceSetup sequenceSetupFromAction = null;
			int setupIndex = -1;
			Action<DefaultSequenceSetup> callback = (sequenceSetup) =>
			{
				sequenceSetupFromAction = sequenceSetup;
				invocationShapeSetups = sequenceSetup.InvocationShapeSetups;
				setupIndex = sequenceSetup.SetupIndex;
			};
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)), callback);
			Assert.Equal(0, setupIndex);
			Assert.Equal("NewMockSequenceFixture.IFoo f => f.Do(1)", sequenceSetupFromAction.Setup.ToString());
			var commonSequenceSetups = invocationShapeSetups.GetSequenceSetups();
			Assert.Single(commonSequenceSetups);
			Assert.Same(sequenceSetupFromAction, commonSequenceSetups[0]);


			var repeatedInvocationShapeSetups = invocationShapeSetups;

			// callback for terminal setups
			aMockSequence.Setup(() =>
			{
				nestedMock.Setup(n => n.ReturnNested(1).DoNested(1));
			}, callback);
			Assert.Equal(1, setupIndex);
			Assert.Equal("NewMockSequenceFixture.INested ... => ....DoNested(1)", sequenceSetupFromAction.Setup.ToString());
			// captures all setups
			Assert.Equal(3, aMockSequence.AllSetups.Count);
			var sequenceSetups = aMockSequence.GetSequenceSetups();
			Assert.Equal(2, sequenceSetups.Count);
			Assert.Same(sequenceSetupFromAction, sequenceSetups[1]);
			Assert.NotSame(repeatedInvocationShapeSetups, invocationShapeSetups);

			//repeatedSetup
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)), callback);
			Assert.Equal(2, setupIndex);
			Assert.Same(repeatedInvocationShapeSetups, invocationShapeSetups);
			Assert.Same(sequenceSetupFromAction, invocationShapeSetups.GetSequenceSetups()[1]);

		}

		[Fact]
		public void MockSequenceBaseFindsProtectedAsMockSetups()
		{
			var mock = new Mock<Protected>();
			var protectedAsMock = mock.Protected().As<ProtectedLike>();
			var aMockSequence = new AMockSequence(false, mock);
			DefaultSequenceSetup sequenceSetup = null;
			aMockSequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(1)), s => sequenceSetup = s);
			Assert.Equal("NewMockSequenceFixture.Protected m => m.ProtectedDo(1)", sequenceSetup.Setup.ToString());
		}

		[Fact]
		public void MockSequenceBaseInvokesConditionWithSequenceSetupDerivation()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(false, mock);
			aMockSequence.Setup(() => mock.Setup(f => f.Do(1)));
			mock.Object.Do(1);
			Assert.Single(aMockSequence.ConditionSequenceSetups);
			var conditionSequenceSetup = aMockSequence.ConditionSequenceSetups[0];
			Assert.IsType<DefaultSequenceSetup>(conditionSequenceSetup);
			Assert.Equal("NewMockSequenceFixture.IFoo f => f.Do(1)", conditionSequenceSetup.Setup.ToString());

			Assert.Equal(0, conditionSequenceSetup.SetupIndex);
			var defaultInvocationShapesSetups = conditionSequenceSetup.InvocationShapeSetupsObject as DefaultInvocationShapeSetups;
			var sequenceSetups = defaultInvocationShapesSetups.GetSequenceSetups();
			Assert.Single(sequenceSetups);
			Assert.Same(conditionSequenceSetup, sequenceSetups[0]);
		}

		[Fact]
		public void MockSequenceBaseShouldReturnTheConditionReturnValueForTheActualCondition()
		{
			var mock = new Mock<IFoo>();
			var mockSequence = new AMockSequence(false, mock);
			DefaultSequenceSetup setup = null;
			mockSequence.Setup(() => mock.Setup(m => m.Do(1)), _setup => setup = _setup);
			var methodCall = setup.SetupInternal as MethodCall;

			Assert.True(methodCall.Condition.IsTrue);
			Assert.Same(setup, mockSequence.ConditionSequenceSetups[0]);

			mockSequence.ConditionReturn = false;
			Assert.False(methodCall.Condition.IsTrue);
			Assert.Same(setup, mockSequence.ConditionSequenceSetups[1]);
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
			Assert.Single(aMockSequence.SequenceInvocations);

			nestedMock.Object.ReturnNested(1).DoNested(1);
			Assert.Equal(3, aMockSequence.SequenceInvocations.Count);

		}

		[Fact]
		public void MockSequenceBaseStrictInvokesStrictnessFailureForUnmatchedInvocations()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(true, mock);
			aMockSequence.CallBaseForStrictFailure = false;
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)));

			mock.Object.Do(1);
			Assert.True(aMockSequence.StrictFailure);
		}

		[Fact]
		public void MockSequenceBaseStrictThrowsStrictSequenceExceptionForInvocationsWithoutSequenceSetup()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(true, mock);
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)));

			var exception = Assert.Throws<StrictSequenceException>(() => mock.Object.Do(1));
			Assert.Equal("Invocation without sequence setup. NewMockSequenceFixture.IFoo.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseStrictThrowsStrictSequenceExceptionForInvocationsWithoutSequenceSetupNested()
		{
			var mock = new Mock<IHaveNested>();
			var aMockSequence = new AMockSequence(true, mock);
			aMockSequence.Setup(() => mock.Setup(m => m.ReturnNested(1).DoNested(1)).Returns(1));

			Assert.Equal(1, mock.Object.ReturnNested(1).DoNested(1));

			var exception = Assert.Throws<StrictSequenceException>(() => mock.Object.ReturnNested(1).DoNested(2));
			Assert.Equal("Invocation without sequence setup. NewMockSequenceFixture.INested.DoNested(2)", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseLooseDoesNotThrowsStrictSequenceExceptionForInvocationsWithoutSequenceSetup()
		{
			var mock = new Mock<IFoo>();
			var aMockSequence = new AMockSequence(false, mock);
			aMockSequence.Setup(() => mock.Setup(m => m.Do(2)));

			mock.Object.Do(1);
		}

		[Fact]
		public void MockSequenceBaseLooseVerifyNoOtherCallsShouldThrowIfInvocationsWithoutSequenceSetups()
		{
			var mock = new Mock<IFoo>();
			var mockSequence = new AMockSequence(false, mock);
			mockSequence.Setup(() => mock.Setup(m => m.Do(2)));
			
			mockSequence.VerifyNoOtherCalls();
			
			mock.Object.Do(1);

			var exception = Assert.Throws<SequenceException>(() => mockSequence.VerifyNoOtherCalls());
			Assert.Equal("Expected no invocations without sequence setup but found 1", exception.Message);
		}

		[Fact]
		public void MockSequenceBaseWorksWithMockAs()
		{
			var mock = new Mock<IFoo>();
			var mockBar = mock.As<IBar>();
			// works without mockBar too
			var mockSequence = new AMockSequence(true, mock, mockBar);
			
			mockSequence.Setup(() => mockBar.Setup(m => m.DoBar(1)).Returns(1));
			mockSequence.Setup(() => mockBar.Setup(m => m.DoBar(2)).Returns(2));

			Assert.Equal(1, mockBar.Object.DoBar(1));
			Assert.Equal(2, mockBar.Object.DoBar(2));

			var exception = Assert.Throws<StrictSequenceException>(() => mockBar.Object.DoBar(0));
			Assert.Equal("Invocation without sequence setup. NewMockSequenceFixture.IBar.DoBar(0)", exception.Message);

		}

		private void LooseNewMockSequenceTestBase(Action<Mock<IFoo>, IProtectedAsMock<Protected, ProtectedLike>, NewMockSequence> setupSequence, Action<IFoo, Protected> act)
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
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
				Assert.Equal(2, mocked.Do(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfNotCalledInOrder_OneTime()
		{
			var exception = Assert.Throws<SequenceException>(() =>
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

			Assert.Equal("Expected invocation on the mock once, but was 0 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfCalledCorrectExactTimes()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Exactly(2));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfCalledLessThanExactTimes()
		{
			var exception = Assert.Throws<SequenceException>(() =>
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

			Assert.Equal("Expected invocation on the mock exactly 2 times, but was 1 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldThrowIfCalledMoreThanExactTimes()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Exactly(2));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);

				})
			);

			Assert.Equal("Expected invocation on the mock exactly 2 times, but was 3 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtLeastTimesIsMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtLeast(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtLeastTimesIsMetMoreThan()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtLeast(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowIfAtLeastTimesIsNotMet()
		{
			var exception = Assert.Throws<SequenceException>(() =>
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

			Assert.Equal("Expected invocation on the mock at least 2 times, but was 1 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldMoveToNextConsecutiveInvocationShapeSetupWhenAtLeastMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				// effectively AtLeast(3) with different returns
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtLeast(2));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2), Times.AtLeast(1));
				mockSequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(9));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				Assert.Equal(9, mocked.Do(2));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfNeverSetupCalled()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)));
					mockSequence.Setup(() => mock.Setup(m => m.Do(2)), Times.Never());
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(2);
				})
			);

			Assert.Equal("Expected invocation on the mock should never have been performed, but was 1 times: NewMockSequenceFixture.IFoo m => m.Do(2)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfAtMostMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfInvokedMoreThanAtMost()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.AtMost(2));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);
				})
			);

			Assert.Equal("Expected invocation on the mock at most 2 times, but was 3 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldMoveToNextConsecutiveInvocationShapeSetupWhenAtMostMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				// effectively AtMost(3) with different returns
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2), Times.AtMost(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				Assert.Throws<SequenceException>(() => mocked.Do(1));
			});
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfBetweenInclusiveMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Between(0, 2, Range.Inclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});

			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Between(0, 2, Range.Inclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});

			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Between(0, 2, Range.Inclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfBetweenInclusiveTooManyInvocations()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(0, 2, Range.Inclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);
				})
			);

			Assert.Equal("Expected invocation on the mock between 0 and 2 times (Inclusive), but was 3 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldThrowIfBetweenInclusiveTooFewInvocations()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(1, 2, Range.Inclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					protectedMocked.InvokeProtectedDo(1);
				})
			);

			Assert.Equal("Expected invocation on the mock between 1 and 2 times (Inclusive), but was 0 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceConsecutiveBetweenInclusiveOrExclusiveCanBeSpecifiedWithAtLeastThenAtMost()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				//equivalent of BetweenInclusive(2,5)
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtLeast(2));
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2), Times.AtMost(3));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				Assert.Equal(2, mocked.Do(1));
				var exception = Assert.Throws<SequenceException>(() => mocked.Do(1));
				Assert.Equal("Expected invocation on the mock at most 3 times, but was 4 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);

				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldNotThrowIfBetweenExclusiveMet()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Between(1, 4, Range.Exclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});

			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.Between(1, 4, Range.Exclusive));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			});
		}

		[Fact]
		public void MockSequenceShouldThrowImmediatelyIfBetweenExclusiveTooManyInvocations()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(1, 3, Range.Exclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					mocked.Do(1);
					mocked.Do(1);
				})
			);

			Assert.Equal("Expected invocation on the mock between 1 and 3 times (Exclusive), but was 3 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceShouldThrowIfBetweenExclusiveTooFewInvocations()
		{
			var exception = Assert.Throws<SequenceException>(() =>
				LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
				{
					mockSequence.Setup(() => mock.Setup(m => m.Do(1)), Times.Between(1, 3, Range.Exclusive));
					mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)));
				}, (mocked, protectedMocked) =>
				{
					mocked.Do(1);
					protectedMocked.InvokeProtectedDo(1);
				})
			);

			Assert.Equal("Expected invocation on the mock between 1 and 3 times (Exclusive), but was 1 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceWillNotThrowWhenSkipOptionalTimesSetups()
		{
			LooseNewMockSequenceTestBase((mock, protectedAs, mockSequence) =>
			{
				mockSequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(1)), NewMockSequence.OptionalTimes());
				mockSequence.Setup(() => protectedAs.Setup(p => p.ProtectedDo(2)), NewMockSequence.OptionalTimes());
				mockSequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));
			}, (mocked, protectedMocked) =>
			{
				Assert.Equal(1, mocked.Do(1));
				Assert.Equal(2, mocked.Do(2));
			});
		}

		[Fact]
		public void MockSequenceWillThrowWhenSkipNonOptionalTimesSetups()
		{
			var exception = Assert.Throws<SequenceException>(() =>
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

			Assert.Equal("Expected invocation on the mock once, but was 0 times: NewMockSequenceFixture.Protected p => p.ProtectedDo(2)", exception.Message);
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
		public void MockSequenceStrictShouldThrowIfCyclicInvocationAndNotCyclic()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var sequence = new NewMockSequence(true, mock);
			sequence.Setup(() => mock.Setup(m => m.Do(1)));
			sequence.Setup(() => mock.Setup(m => m.Do(2)));
			mocked.Do(1);
			mocked.Do(2);
			var exception = Assert.Throws<StrictSequenceException>(() => mocked.Do(1));
			Assert.Equal("Cyclical invocation but not cyclic. NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void MockSequenceLooseShouldNotThrowIfCyclicInvocationAndNotCyclic()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var sequence = new NewMockSequence(false, mock);
			sequence.Setup(() => mock.Setup(m => m.Do(1)));
			sequence.Setup(() => mock.Setup(m => m.Do(2)));
			mocked.Do(1);
			mocked.Do(2);
			mocked.Do(1);
		}

		[Fact]
		public void MockSequenceWorksCyclical_OneTime()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();
			var protectedMocked = protectedMock.Object;
			var sequence = new NewMockSequence(true, mock, protectedMock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(2)).Returns(2));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(2, protectedMocked.InvokeProtectedDo(2));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(2, protectedMocked.InvokeProtectedDo(2));
		}

		[Fact]
		public void MockSequenceWorksCyclicalWithSeparatedCommonSetups()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();
			var protectedMocked = protectedMock.Object;

			var sequence = new NewMockSequence(true, mock, protectedMock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(1)).Returns(1));
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(2)).Returns(2));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(2, protectedMocked.InvokeProtectedDo(2));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(2, protectedMocked.InvokeProtectedDo(2));
		}

		[Fact]
		public void MockSequenceWorksCyclicalWithAdjacentCommonSetups()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();
			var protectedMocked = protectedMock.Object;

			var sequence = new NewMockSequence(true, mock, protectedMock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(1)).Returns(1));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
		}

		[Fact]
		public void MockSequenceWorksCyclicalWithAdjacentStartEnd()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();
			var protectedMocked = protectedMock.Object;

			var sequence = new NewMockSequence(true, mock, protectedMock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(1)).Returns(1));
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
		}

		[Fact]
		public void MockSequenceWorksCyclicalWithAdjacentStartEndExactTimesMoreThanOnce()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			var protectedMock = new Mock<Protected>();
			var protectedAsMock = protectedMock.Protected().As<ProtectedLike>();
			var protectedMocked = protectedMock.Object;

			var sequence = new NewMockSequence(true, mock, protectedMock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => protectedAsMock.Setup(m => m.ProtectedDo(1)).Returns(1));
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(2), Times.Exactly(2));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, protectedMocked.InvokeProtectedDo(1));
			Assert.Equal(2, mocked.Do(1));
			Assert.Equal(2, mocked.Do(1));
		}

		[Fact]
		public void MockSequenceCyclicalBeforeCurrentAfterBeginningShouldThrowIfNotOptional()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1));
			sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));
			sequence.Setup(() => mock.Setup(m => m.Do(3)).Returns(3));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(2, mocked.Do(2));
			Assert.Equal(3, mocked.Do(3));

			var exception = Assert.Throws<SequenceException>(() => Assert.Equal(2, mocked.Do(2)));
			Assert.Equal("Expected invocation on the mock once, but was 0 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		[Fact]
		public void VerifiableSetupHasCyclicalExecutionCount()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			Assert.Equal(1, mocked.Do(1));
			Assert.Equal(1, mocked.Do(1));

			Assert.Equal(new List<int> { 2 }, verifiableSequence1.CyclicalExecutionCount);
			Assert.Equal(new List<int> { 0 }, verifiableSequence2.CyclicalExecutionCount);

			Assert.Equal(2, mocked.Do(2));
			Assert.Equal(1, mocked.Do(1));

			Assert.Equal(new List<int> { 2, 1 }, verifiableSequence1.CyclicalExecutionCount);
			Assert.Equal(new List<int> { 1, 0 }, verifiableSequence2.CyclicalExecutionCount);
		}

		[Fact]
		public void VerifiableSetupVerifyCyclicalNumberWillThrowIfDifferentCycles()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			verifiableSequence1.VerifyCyclical(new List<int> { 0 });
			var exception = Assert.Throws<SequenceException>(() => verifiableSequence1.VerifyCyclical(new List<int> { 0, 0 }));
			Assert.Equal("Expected cycles 2 but was 1", exception.Message);
		}


		[Fact]
		public void VerifiableSetupVerifyCyclicalTimesWillThrowIfDifferentCycles()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			var exception = Assert.Throws<SequenceException>(() => verifiableSequence1.VerifyCyclical(new List<Times> { Times.Once(), Times.Once(), Times.Once() }));
			Assert.Equal("Expected cycles 3 but was 1", exception.Message);
		}

		[Fact]
		public void VerifiableSetupVerifyCyclicalNumberWillThrowIfDiffers()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			mocked.Do(1);

			var exception = Assert.Throws<SequenceException>(() => verifiableSequence1.VerifyCyclical(new List<int> { 2 }));
			Assert.Equal("On cycle 0. Expected invocation on the mock exactly 2 times, but was 1 times: ", exception.Message);
		}

		[Fact]
		public void VerifiableSetupVerifyCyclicalTimesWillThrowIfDiffers()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			mocked.Do(1);
			mocked.Do(2);
			mocked.Do(1);

			var exception = Assert.Throws<SequenceException>(() => verifiableSequence1.VerifyCyclical(new List<Times> { Times.Once(), Times.Exactly(2) }));
			Assert.Equal("On cycle 1. Expected invocation on the mock exactly 2 times, but was 1 times: ", exception.Message);
		}

		[Fact]
		public void VerifySetupVerifyWithTimesNumberCanBeUsedForCyclical()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;

			var sequence = new NewMockSequence(true, mock) { Cyclical = true };
			var verifiableSequence1 = sequence.Setup(() => mock.Setup(m => m.Do(1)).Returns(1), Times.AtMost(2));
			var verifiableSequence2 = sequence.Setup(() => mock.Setup(m => m.Do(2)).Returns(2));

			mocked.Do(1);
			verifiableSequence1.Verify(1);
			mocked.Do(1);
			verifiableSequence1.Verify(2);
			mocked.Do(2);
			mocked.Do(1);
			verifiableSequence1.Verify(3);

			var exception = Assert.Throws<SequenceException>(() => verifiableSequence1.Verify(4));
			Assert.Equal("Expected invocation on the mock exactly 4 times, but was 3 times: NewMockSequenceFixture.IFoo m => m.Do(1)", exception.Message);
		}

		

		internal class DefaultSequenceSetup : SequenceSetupBase
		{
			public DefaultInvocationShapeSetups InvocationShapeSetups => (DefaultInvocationShapeSetups)InvocationShapeSetupsObject;
		}

		internal class DefaultInvocationShapeSetups : InvocationShapeSetupsBase<DefaultSequenceSetup>
		{
			public DefaultInvocationShapeSetups(DefaultSequenceSetup sequenceSetup) : base(sequenceSetup) { }

			public IReadOnlyList<DefaultSequenceSetup> GetSequenceSetups()
			{
				return SequenceSetups;
			}
		}

		internal class AMockSequence : MockSequenceBase<DefaultSequenceSetup, DefaultInvocationShapeSetups>
		{
			public bool StrictFailure { get; set; }
			public bool CallBaseForStrictFailure { get; set; } = true;
			public bool ConditionReturn { get; set; } = true;
			public AMockSequence(bool strict, params Mock[] mocks) : base(strict, mocks) { }
			public void Setup(Action setup, Action<DefaultSequenceSetup> setupCallback = null)
			{
				if(setupCallback == null)
				{
					setupCallback = s => { };
				}
				base.InterceptSetup(setup, setupCallback);
			}

			public List<DefaultSequenceSetup> ConditionSequenceSetups = new List<DefaultSequenceSetup>();

			protected override bool Condition(DefaultSequenceSetup sequenceSetup)
			{
				ConditionSequenceSetups.Add(sequenceSetup);
				return ConditionReturn;
			}

			internal IReadOnlyList<DefaultSequenceSetup> GetSequenceSetups()
			{
				return SequenceSetups;
			}

			protected override void StrictnessFailure(ISequenceInvocation unmatchedInvocation)
			{
				StrictFailure = true;
				if (CallBaseForStrictFailure)
				{
					base.StrictnessFailure(unmatchedInvocation);
				}
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

		public interface IBar
		{
			int DoBar(int arg);
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
