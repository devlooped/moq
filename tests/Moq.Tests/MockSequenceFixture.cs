// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public void WorksWithProtectedAsSuccess()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);
			protectedAs.InSequence(sequence).Setup(y => y.ProtectedDo(300)).Returns(301);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Equal(201, a.Object.Do(200));
			Assert.Equal(301, protectedAs.Mocked.Object.InvokeProtectedDo(300));

		}

		[Fact]
		public void WorksWithProtectedAsFailure()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();

			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
			protectedAs.InSequence(sequence).Setup(y => y.ProtectedDo(300)).Returns(301);
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);

			Assert.Equal(101, a.Object.Do(100));
			Assert.Throws<MockException>(() => a.Object.Do(200));

		}

		[Fact]
		public void VerifySequenceVerifyAllVerifiesAllSetupsRegardlessOfMarkedAsVerified()
		{
			void PerformTest(Action<IFoo,Protected> act)
			{
				var a = new Mock<IFoo>(MockBehavior.Strict);
				var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
				var sequence = new MockSequence();
				a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
				a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201);
				protectedAs.InSequence(sequence).Setup(y => y.ProtectedDo(300)).Returns(301);

				act(a.Object, protectedAs.Mocked.Object);
				sequence.VerifySequence(true);
			}

			PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
				@protected.InvokeProtectedDo(300);
			});

			Assert.Throws<MockException>(() => PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
			}));

		}

		[Fact]
		public void VerifySequenceVerifiesVerifiableSetups()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			var sequence = new MockSequence();
			a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101).Verifiable();
			a.InSequence(sequence).Setup(x => x.Do(200)).Returns(201).Verifiable();
			protectedAs.InSequence(sequence).Setup(y => y.ProtectedDo(300)).Returns(301);

			a.Object.Do(100);
			a.Object.Do(200);

			sequence.VerifySequence(false);

		}

		[Fact]
		public void VerifySequencesVerifyAllVerifiesAllSetupsInAllSequencesRegardlessOrVerifiable()
		{
			void PerformTest(Action<IFoo, Protected> act)
			{
				var a = new Mock<IFoo>(MockBehavior.Strict);
				var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
				var sequence1 = new MockSequence();
				a.InSequence(sequence1).Setup(x => x.Do(100)).Returns(101).Verifiable();
				a.InSequence(sequence1).Setup(x => x.Do(200)).Returns(201).Verifiable();
				protectedAs.InSequence(sequence1).Setup(y => y.ProtectedDo(300)).Returns(301);

				var sequence2 = new MockSequence();
				a.InSequence(sequence2).Setup(x => x.Do(1)).Returns(101).Verifiable();
				a.InSequence(sequence2).Setup(x => x.Do(2)).Returns(201).Verifiable();
				protectedAs.InSequence(sequence2).Setup(y => y.ProtectedDo(3)).Returns(301);

				act(a.Object, protectedAs.Mocked.Object);
				VerifiableSequence.VerifySequences(true, sequence1, sequence2);
			}

			PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
				foo.Do(1);
				@protected.InvokeProtectedDo(300);
				foo.Do(2);
				@protected.InvokeProtectedDo(3);
			});

			Assert.Throws<MockException>(() => PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
				foo.Do(1);
				@protected.InvokeProtectedDo(300);
				foo.Do(2);
			}));

		}

		[Fact]
		public void VerifySequencesVerifiesAllVerifiableSetupsInAllSequences()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			var sequence1 = new MockSequence();
			a.InSequence(sequence1).Setup(x => x.Do(100)).Returns(101).Verifiable();
			a.InSequence(sequence1).Setup(x => x.Do(200)).Returns(201).Verifiable();
			protectedAs.InSequence(sequence1).Setup(y => y.ProtectedDo(300)).Returns(301);

			var sequence2 = new MockSequence();
			a.InSequence(sequence2).Setup(x => x.Do(1)).Returns(101).Verifiable();
			a.InSequence(sequence2).Setup(x => x.Do(2)).Returns(201).Verifiable();
			protectedAs.InSequence(sequence2).Setup(y => y.ProtectedDo(3)).Returns(301);

			foo.Do(100);
			foo.Do(200);
			foo.Do(1);
			protectedAs.Mocked.Object.InvokeProtectedDo(300);
			foo.Do(2);

			VerifiableSequence.VerifySequences(false, sequence1, sequence2);
		}

		[Fact]
		public void VerifySequenceAndMocksVerifyAllVerifiesTheSequenceAndTheMocksRegardlessOfVerifiable()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			a.Setup(f => f.Do(1)).Returns(1);
			var sequence = new MockSequence();
			Assert.Throws<MockException>(() => sequence.VerifySequenceAndMocks(true, a));

			a.Object.Do(1);
			sequence.VerifySequenceAndMocks(true, a);

			a.InSequence(sequence).Setup(f => f.Do(2)).Returns(2);

			Assert.Throws<MockException>(() => sequence.VerifySequenceAndMocks(true, a));
		}

		[Fact]
		public void VerifySequenceAndMocksVerifiesVerfiableSetupsOfTheSequenceAndTheMocks()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			a.Setup(f => f.Do(1)).Returns(1);
			var sequence = new MockSequence();
			sequence.VerifySequenceAndMocks(false, a);

			a.InSequence(sequence).Setup(f => f.Do(2)).Returns(2);

			sequence.VerifySequenceAndMocks(false, a);
		}

		[Fact]
		public void VerifySequenceAndAllMocksVerifyAllVerifiesTheSequenceAndAllMocksOfTheSequenceRegardlessOfVerifiable()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			a.Setup(f => f.Do(1)).Returns(1);
			var sequence = new MockSequence();
			// does not throw as the mock is not in the sequence
			sequence.VerifySequenceAndAllMocks(true);

			a.InSequence(sequence).Setup(f => f.Do(2)).Returns(2);
			a.Object.Do(2);

			// throws for the mock
			Assert.Throws<MockException>(() => sequence.VerifySequenceAndAllMocks(true));

			a.Object.Do(1);
			sequence.VerifySequenceAndAllMocks(true);

			a.InSequence(sequence).Setup(f => f.Do(3)).Returns(3);

			// throws for the sequence
			Assert.Throws<MockException>(() => sequence.VerifySequenceAndAllMocks(true));
		}

		[Fact]
		public void VerifySequenceAndAllMocksVerifiesVerifiableSetupsOfTheSequenceAndAllMocksOfTheSequence()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			a.Setup(f => f.Do(1)).Returns(1).Verifiable();
			a.Setup(f => f.Do(0)).Returns(0);
			var sequence = new MockSequence();
			// does not throw as the mock is not in the sequence
			sequence.VerifySequenceAndAllMocks(false);

			// throws for the mock
			a.InSequence(sequence).Setup(f => f.Do(2)).Returns(2);
			Assert.Throws<MockException>(() => sequence.VerifySequenceAndAllMocks(false));

			a.Object.Do(1);
			sequence.VerifySequenceAndAllMocks(false);

			a.InSequence(sequence).Setup(f => f.Do(3)).Returns(3).Verifiable();

			// throws for the sequence
			Assert.Throws<MockException>(() => sequence.VerifySequenceAndAllMocks(false));
		}

		[Fact]
		public void VerifySequencesAndAllMocksVerifyAllVerifiesAllSequencesAndAllTheirMocksRegardlessOfVerifiable()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			a.Setup(f => f.Do(1)).Returns(1);
			var sequence1 = new MockSequence();
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			var @protected = protectedAs.Mocked.Object;
			protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1);
			var sequence2 = new MockSequence();

			void PerformVerification()
			{
				VerifiableSequence.VerifySequencesAndAllMocks(true, sequence1, sequence2);
			}

			// does not throw as no mocks in the sequence
			PerformVerification();
			
			a.InSequence(sequence1).Setup(f => f.Do(2)).Returns(2);
			foo.Do(2);

			// throws for the mock
			Assert.Throws<MockException>(PerformVerification);

			foo.Do(1);
			PerformVerification();

			protectedAs.InSequence(sequence2).Setup(p => p.ProtectedDo(2)).Returns(2);
			@protected.InvokeProtectedDo(2);

			// throws for the mock
			Assert.Throws<MockException>(PerformVerification);

			@protected.InvokeProtectedDo(1);
			PerformVerification();

			a.InSequence(sequence1).Setup(f => f.Do(3)).Returns(3);
			// throws for sequences
			Assert.Throws<MockException>(PerformVerification);

			foo.Do(3);
			PerformVerification();

			protectedAs.InSequence(sequence2).Setup(p => p.ProtectedDo(3)).Returns(3);
			// throws for sequences
			Assert.Throws<MockException>(PerformVerification);

			@protected.InvokeProtectedDo(3);
			PerformVerification();

		}

		[Fact]
		public void VerifySequencesAndAllMocksVerifiesAllVerifiableSetupsAllSequencesAndAllTheirMocks()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			a.Setup(f => f.Do(1)).Returns(1).Verifiable();
			var sequence1 = new MockSequence();
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			var @protected = protectedAs.Mocked.Object;
			protectedAs.Setup(p => p.ProtectedDo(1)).Returns(1);
			var sequence2 = new MockSequence();

			void PerformVerification()
			{
				VerifiableSequence.VerifySequencesAndAllMocks(false, sequence1, sequence2);
			}

			// no mocks in sequences
			PerformVerification();

			a.InSequence(sequence1).Setup(f => f.Do(2)).Returns(2);

			//throws for mock
			Assert.Throws<MockException>(PerformVerification);

			foo.Do(1);
			PerformVerification();

			protectedAs.InSequence(sequence2).Setup(p => p.ProtectedDo(2)).Returns(2).Verifiable();
			@protected.InvokeProtectedDo(2);
			// does not throw as mock setup is not verifiable
			PerformVerification();

			a.InSequence(sequence1).Setup(f => f.Do(3)).Returns(3).Verifiable();
			//throws for sequence
			Assert.Throws<MockException>(PerformVerification);

			foo.Do(2);//satisfy the sequence
			foo.Do(3);
			PerformVerification();

		}

		[Fact]
		public void VerifiableSequencePermitsDifferentSequenceLogic()
		{
			void PerformTest(Action<IFoo, Protected> act)
			{
				var a = new Mock<IFoo>(MockBehavior.Strict);
				var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
				var sequence = new ConsecutiveInvocationsSequence();
				a.InSequence(sequence).Setup(x => x.Do(100)).Returns(101);
				a.InSequence(sequence.ConsecutiveInvocations(2)).Setup(x => x.Do(200)).Returns(201);
				protectedAs.InSequence(sequence.ConsecutiveInvocations(3)).Setup(y => y.ProtectedDo(300)).Returns(301);
				protectedAs.InSequence(sequence).Setup(y => y.ProtectedDo(400)).Returns(401);
				act(a.Object, protectedAs.Mocked.Object);
			}

			PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
				foo.Do(200);
				@protected.InvokeProtectedDo(300);
				@protected.InvokeProtectedDo(300);
				@protected.InvokeProtectedDo(300);
				@protected.InvokeProtectedDo(400);
			});

			Assert.Throws<MockException>(() => PerformTest((foo, @protected) =>
			{
				foo.Do(100);
				foo.Do(200);
				foo.Do(200);
				foo.Do(200);
			}));
		}

		[Fact]
		public void VerifyExtensionVerifiesVerifiableSetupsOfTheMockAndAllSetupsApplicableToTheMockFromTheSequences()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			var seq = new MockSequence();
			a.Setup(f => f.Do(0)).Returns(0).Verifiable();
			a.InSequence(seq).Setup(f => f.Do(1)).Returns(1).Verifiable();
			a.InSequence(seq).Setup(f => f.Do(2)).Returns(2).Verifiable();
			
			Assert.Throws<MockException>(() => a.Verify(seq));
			foo.Do(0);
			Assert.Throws<MockException>(() => a.Verify(seq));
			foo.Do(1);
			Assert.Throws<MockException>(() => a.Verify(seq));
			foo.Do(2);
			a.Verify(seq);

			a.Setup(f => f.Do(3)).Returns(3);
			a.Verify(seq);

			var seq2 = new MockSequence();
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			protectedAs.InSequence(seq2).Setup(p => p.ProtectedDo(0)).Returns(0).Verifiable();
			a.InSequence(seq2).Setup(f => f.Do(4)).Returns(4);

			a.Verify(seq, seq2);
		}

		[Fact]
		public void VerifyAllExtensionVerifiesAllSetupsOfTheMockAndAllSetupsApplicableToTheMockFromTheSequences()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			var seq = new MockSequence();
			a.Setup(f => f.Do(0)).Returns(0);
			a.InSequence(seq).Setup(f => f.Do(1)).Returns(1);
			a.InSequence(seq).Setup(f => f.Do(2)).Returns(2);

			Assert.Throws<MockException>(() => a.VerifyAll(seq));
			foo.Do(0);
			Assert.Throws<MockException>(() => a.VerifyAll(seq));
			foo.Do(1);
			Assert.Throws<MockException>(() => a.VerifyAll(seq));
			foo.Do(2);
			a.VerifyAll(seq);

			a.Setup(f => f.Do(3)).Returns(3);
			Assert.Throws<MockException>(() => a.VerifyAll(seq));
			foo.Do(3);

			var seq2 = new MockSequence();
			var protectedAs = new Mock<Protected>().Protected().As<ProtectedLike>();
			protectedAs.InSequence(seq2).Setup(p => p.ProtectedDo(0)).Returns(0);
			a.InSequence(seq2).Setup(f => f.Do(4)).Returns(4);

			Assert.Throws<MockException>(() => a.VerifyAll(seq, seq2));
			protectedAs.Mocked.Object.InvokeProtectedDo(0);
			Assert.Throws<MockException>(() => a.VerifyAll(seq, seq2));
			foo.Do(4);
			a.VerifyAll(seq, seq2);
		}

		[Fact]
		public void VerifiableSequencesCanProvideCustomVerifySequenceLogic()
		{
			var a = new Mock<IFoo>(MockBehavior.Strict);
			var foo = a.Object;
			var seq = new CustomVerificationSequence();
			a.InSequence(seq).Setup(f => f.Do(1)).Returns(1);
			a.InSequence(seq).Setup(f => f.Do(2)).Returns(2);

			foo.Do(1);
			foo.Do(2);
			foo.Do(1);

			Assert.Throws<Exception>(() => seq.VerifySequence(true));
			foo.Do(2);
			seq.VerifySequence(true);

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

		public interface IFoo
		{
			int Do(int arg);
		}

		public class ConsecutiveInvocationsSequence : VerifiableSequence
		{
			private int consecutiveInvocations = 1;
			private Queue<int> order = new Queue<int>();
			
			public ConsecutiveInvocationsSequence ConsecutiveInvocations(int consecutiveInvocations)
			{
				this.consecutiveInvocations = consecutiveInvocations;
				return this;
			}
			
			protected override bool ConditionImpl(ITrackedSetup trackedSetup)
			{
				var expectedSetupIndex = order.Dequeue();
				return expectedSetupIndex == trackedSetup.SetupIndex;
			}

			protected override void AddedSetup(ITrackedSetup trackedSetup)
			{
				for(var i = 0; i < consecutiveInvocations; i++)
				{
					order.Enqueue(trackedSetup.SetupIndex);
				}
				
				consecutiveInvocations = 1;
			}
		}

		public class CustomVerificationSequence : VerifiableSequence
		{
			private int CompletedCycles = 2;
			
			int expectedSetupIndex;

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			protected override bool ConditionImpl(ITrackedSetup trackedSetup)
			{
				var setupIndex = trackedSetup.SetupIndex;
				if (setupIndex == expectedSetupIndex)
				{
					if (expectedSetupIndex == numberOfSetups - 1)
					{
						expectedSetupIndex = 0;
					}
					else
					{
						expectedSetupIndex++;
					}

					return true;
				}
				return false;
			}

			public override void VerifySequence(bool verifyAll = true)
			{
				base.VerifySequence(verifyAll);
				foreach(var trackedSetup in GetCurrentSetups())
				{
					if((verifyAll || trackedSetup.Setup.IsVerifiable) && trackedSetup.ExecutionIndices.Count != CompletedCycles)
					{
						throw new Exception($"Setup did not participate in {CompletedCycles} cycles");
					}
				}
			}
		}
	}
}
