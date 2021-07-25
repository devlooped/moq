// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq.Language;
using Moq.Language.Flow;
using Moq.Protected;

namespace Moq
{
	/// <summary>
	/// Helper class to setup a full trace between many mocks
	/// </summary>
	public class MockSequence : VerifiableSequence
	{
		int expectedSequenceIndex;

		/// <summary>
		/// Initialize a trace setup
		/// </summary>
		public MockSequence()
		{
			expectedSequenceIndex = 0;
		}

		/// <summary>
		/// Allow sequence to be repeated
		/// </summary>
		public bool Cyclic { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceIndex"></param>
		/// <returns></returns>
		public override bool Condition(int sequenceIndex)
		{
			if(sequenceIndex == expectedSequenceIndex)
			{
				if(expectedSequenceIndex == numberOfSetups - 1 && Cyclic)
				{
					expectedSequenceIndex = 0;
				}
				else
				{
					expectedSequenceIndex++;
				}
				
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceIndex"></param>
		public override void SequenceSetupExecuted(int sequenceIndex)
		{
		}
	}

	/// <summary>
	/// Contains extension methods that are related to <see cref="MockSequence"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockSequenceHelper
	{
		/// <summary>
		///  todo
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <param name="mock"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static ISequenceSetupConditionResult<TMock,TMock> InSequence<TMock>(
			this Mock<TMock> mock,
			VerifiableSequence sequence)
			where TMock : class
		{
			Guard.NotNull(sequence, nameof(sequence));

			return sequence.For(mock);
		}

		/// <summary>
		/// Perform an expectation in the trace.
		/// </summary>
		public static ISequenceSetupConditionResult<TMock, TAnalog> InSequence<TMock, TAnalog>(
			this IProtectedAsMock<TMock, TAnalog> protectedAsMock,
			VerifiableSequence sequence)
			where TMock : class
			where TAnalog : class
		{
			Guard.NotNull(sequence, nameof(sequence));

			return sequence.For(protectedAsMock);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal interface IMockSequence
	{
		/// <summary>
		/// 
		/// </summary>
		int SetupIndex { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="index"></param>
		/// <param name="mock"></param>
		void AddSetup(ISetup setup, int index,Mock mock);
		
		bool Condition(int index);
		
		void SequenceSetupExecuted(int index);
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class VerifiableSequence : IMockSequence
	{
		/// <summary>
		/// 
		/// </summary>
		public int SetupIndex { get; set; }

		private Dictionary<Mock, List<Moq.ISetup>> mockSetups = new Dictionary<Mock, List<ISetup>>();
		/// <summary>
		/// 
		/// </summary>
		protected int numberOfSetups = 0;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="index"></param>
		/// <param name="mock"></param>
		public void AddSetup(ISetup setup, int index, Mock mock)
		{
			numberOfSetups++;
			if (!mockSetups.TryGetValue(mock, out var setups))
			{
				setups = new List<Moq.ISetup>();
				mockSetups.Add(mock, setups);
			}
			setups.Add(setup);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		public void VerifySequence(bool verifyAll = true)
		{
			VerifySetups(GetCurrentSetups(), verifyAll);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		/// <param name="sequences"></param>
		public static void VerifySequences(bool verifyAll,params VerifiableSequence[] sequences)
		{
			foreach(var sequence in sequences)
			{
				sequence.VerifySequence(verifyAll);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		/// <param name="verifyAll"></param>
		public void VerifySequenceAndMock(Mock mock, bool verifyAll = true)
		{
			VerifySequenceAndMocks(new Mock[] { mock }, verifyAll);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		public void VerifySequenceAndAllMocks(bool verifyAll = true)
		{
			VerifySequenceAndMocks(mockSetups.Keys.ToArray(), verifyAll);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		/// <param name="sequences"></param>
		public static void VerifySequencesAndAllMocks(bool verifyAll, params VerifiableSequence[] sequences)
		{
			foreach (var sequence in sequences)
			{
				sequence.VerifySequenceAndAllMocks(verifyAll);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		public void MockVerify(Mock mock)
		{
			VerifySetups(GetCurrentSetups(mock), false);
			mock.Verify();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		public void MockVerifyAll(Mock mock)
		{
			VerifySetups(GetCurrentSetups(mock), true);
			mock.VerifyAll();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setups"></param>
		/// <param name="verifyAll"></param>
		private void VerifySetups(IEnumerable<Moq.ISetup> setups, bool verifyAll)
		{
			foreach (var setupForVerification in setups)
			{
				if (verifyAll)
				{
					setupForVerification.VerifyAll();
				}
				else
				{
					if (setupForVerification.IsVerifiable)
					{
						setupForVerification.Verify();//using recursive true as that is what Mock.Verify()/VerifyAll() does
					}
				}
			}
		}

		private void VerifySequenceAndMocks(Mock[] mocks, bool verifyAll)
		{
			VerifySequence(verifyAll);

			if (verifyAll)
			{
				Mock.VerifyAll(mocks);
			}
			else
			{
				Mock.Verify(mocks);
			}
		}

		private List<Moq.ISetup> GetCurrentSetups()
		{
			List<Moq.ISetup> currentSetups = new List<ISetup>();
			foreach (var entry in mockSetups)
			{
				currentSetups.AddRange(GetCurrentSetups(entry.Key, entry.Value));
			}
			return currentSetups;
		}

		private List<ISetup> GetCurrentSetups(Mock mock, List<ISetup> storedSetups)
		{
			var currentMockSetups = mock.MutableSetups;
			var removals = new List<Moq.ISetup>();
			foreach (var storedSetup in storedSetups)
			{
				if (!currentMockSetups.Contains(storedSetup))
				{
					removals.Add(storedSetup);
				}
			}
			foreach (var removal in removals)
			{
				storedSetups.Remove(removal);
			}
			return storedSetups;
		}

		private List<ISetup> GetCurrentSetups(Mock mock)
		{
			if (mockSetups.ContainsKey(mock))
			{
				return GetCurrentSetups(mock, mockSetups[mock]);
			}
			else
			{
				throw new ArgumentException("Mock is not in a sequence");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceIndex"></param>
		/// <returns></returns>
		public abstract bool Condition(int sequenceIndex);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceIndex"></param>
		/// <returns></returns>
		public abstract void SequenceSetupExecuted(int sequenceIndex);

		internal ISequenceSetupConditionResult<TMock,TMock> For<TMock>(Mock<TMock> mock)
			where TMock : class
		{
			return new MockSequencePhrase<TMock>(mock, this);
		}

		internal ISequenceSetupConditionResult<TMock, TAnalog> For<TMock,TAnalog>(IProtectedAsMock<TMock, TAnalog> mock)
			where TMock : class
			where TAnalog : class
		{
			return new MockProtectedSequencePhrase<TMock,TAnalog>(mock, this);
		}
	}

}
