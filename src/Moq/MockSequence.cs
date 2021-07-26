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
		int expectedSetupIndex;

		/// <summary>
		/// Initialize a trace setup
		/// </summary>
		public MockSequence()
		{
			expectedSetupIndex = 0;
		}

		/// <summary>
		/// Allow sequence to be repeated
		/// </summary>
		public bool Cyclic { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override bool ConditionImpl(ITrackedSetup trackedSetup)
		{
			var setupIndex = trackedSetup.SetupIndex;
			if(setupIndex == expectedSetupIndex)
			{
				if(expectedSetupIndex == numberOfSetups - 1 && Cyclic)
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
		/// <param name="setupIndex"></param>
		/// <param name="mock"></param>
		void AddSetup(ISetup setup, int setupIndex,Mock mock);
		
		bool Condition(int index);
		
		void SequenceSetupExecuted(int setupIndex);
	}

	/// <summary>
	/// 
	/// </summary>
	public static class VerifiableSequenceExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		/// <param name="sequences"></param>
		public static void Verify(this Mock mock, params VerifiableSequence[] sequences)
		{
			foreach (var sequence in sequences)
			{
				sequence.VerifySetupsForMock(mock, false);
			}
			mock.Verify();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		/// <param name="sequences"></param>
		public static void VerifyAll(this Mock mock, params VerifiableSequence[] sequences)
		{
			foreach (var sequence in sequences)
			{
				sequence.VerifySetupsForMock(mock, true);
			}
			mock.VerifyAll();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public interface ITrackedSetup
	{
		/// <summary>
		/// 
		/// </summary>
		ISetup Setup { get; }
		/// <summary>
		/// 
		/// </summary>
		int SetupIndex { get; }
		/// <summary>
		/// 
		/// </summary>
		IReadOnlyList<int> ExecutionIndices { get; }

		/// <summary>
		/// 
		/// </summary>
		Mock Mock { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class VerifiableSequence : IMockSequence
	{
		private class TrackedSetup : ITrackedSetup
		{
			public ISetup Setup { get; set; }

			public int SetupIndex { get; set; }

			public List<int> ExecutionIndicesList { get; } = new List<int>();

			public IReadOnlyList<int> ExecutionIndices => ExecutionIndicesList;

			public Mock Mock { get; set; }
		}

		private int executionCount = 0;
		/// <summary>
		/// 
		/// </summary>
		protected bool ThrowIfConditionNotMetWhenLoose { get; set; } = true;

		/// <summary>
		/// 
		/// </summary>
		public int SetupIndex { get; set; }

		private Dictionary<Mock, List<TrackedSetup>> trackedSetups = new Dictionary<Mock, List<TrackedSetup>>();
		
		/// <summary>
		/// 
		/// </summary>
		protected int numberOfSetups = 0;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="setupIndex"></param>
		/// <param name="mock"></param>
		public void AddSetup(ISetup setup, int setupIndex, Mock mock)
		{
			numberOfSetups++;
			if (!trackedSetups.TryGetValue(mock, out var setups))
			{
				setups = new List<TrackedSetup>();
				trackedSetups.Add(mock, setups);
			}
			var trackedSetup = new TrackedSetup { Setup = setup, SetupIndex = setupIndex, Mock = mock };
			setups.Add(new TrackedSetup { Setup = setup, SetupIndex = setupIndex, Mock = mock});
			AddedSetup(trackedSetup);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		protected virtual void AddedSetup(ITrackedSetup trackedSetup)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		public virtual void VerifySequence(bool verifyAll = true)
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
		/// <param name="mocks"></param>
		/// <param name="verifyAll"></param>
		public void VerifySequenceAndMocks(bool verifyAll,params Mock[] mocks)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verifyAll"></param>
		public void VerifySequenceAndAllMocks(bool verifyAll = true)
		{
			VerifySequenceAndMocks(verifyAll, trackedSetups.Keys.ToArray());
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
			VerifySetupsForMock(mock, false);
			mock.Verify();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mock"></param>
		public void MockVerifyAll(Mock mock)
		{
			VerifySetupsForMock(mock, true);
			mock.VerifyAll();
		}

		internal void VerifySetupsForMock(Mock mock, bool verifyAll)
		{
			VerifySetups(GetCurrentSetups(mock), verifyAll);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setups"></param>
		/// <param name="verifyAll"></param>
		protected void VerifySetups(IEnumerable<ITrackedSetup> setups, bool verifyAll)
		{
			foreach (var setupForVerification in setups.Select( s=> s.Setup))
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected List<ITrackedSetup> GetCurrentSetups()
		{
			List<ITrackedSetup> currentSetups = new List<ITrackedSetup>();
			foreach (var entry in trackedSetups)
			{
				currentSetups.AddRange(GetCurrentSetups(entry.Key, entry.Value));
			}
			return currentSetups;
		}

		private List<TrackedSetup> GetCurrentSetups(Mock mock, List<TrackedSetup> storedSetups)
		{
			var currentMockSetups = mock.MutableSetups;
			var removals = new List<TrackedSetup>();
			foreach (var storedSetup in storedSetups)
			{
				if (!currentMockSetups.Contains(storedSetup.Setup))
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

		private List<TrackedSetup> GetCurrentSetups(Mock mock)
		{
			if (trackedSetups.ContainsKey(mock))
			{
				return GetCurrentSetups(mock, trackedSetups[mock]);
			}
			else
			{
				throw new ArgumentException("Mock is not in a sequence");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setupIndex"></param>
		/// <returns></returns>
		public bool Condition(int setupIndex)
		{
			var trackedSetup = GetTrackedSetup(setupIndex);
			var pass = ConditionImpl(trackedSetup);
			if(!pass && ThrowIfConditionNotMetWhenLoose && trackedSetup.Mock.Behavior == MockBehavior.Loose)
			{
				throw new Exception("todo");//todo MockException
			}
			return pass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		/// <returns></returns>
		protected abstract bool ConditionImpl(ITrackedSetup trackedSetup);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setupIndex"></param>
		/// <returns></returns>
		public void SequenceSetupExecuted(int setupIndex)
		{
			TrackedSetup trackedSetup = GetTrackedSetup(setupIndex);
			trackedSetup.ExecutionIndicesList.Add(executionCount);
			executionCount++;
			SequenceSetupExecutedImpl(trackedSetup);
		}

		private TrackedSetup GetTrackedSetup(int setupIndex)
		{
			TrackedSetup trackedSetup = null;
			foreach (var setups in trackedSetups.Values)
			{
				var found = false;
				foreach (var setup in setups)
				{
					if (setup.SetupIndex == setupIndex)
					{
						trackedSetup = setup;
						found = true;
						break;
					}
				}
				if (found)
				{
					break;
				}
			}
			return trackedSetup;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		protected virtual void SequenceSetupExecutedImpl(ITrackedSetup trackedSetup)
		{

		}

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
