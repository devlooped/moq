// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TSequenceSetup"></typeparam>
	public abstract class InvocationShapeSetupsBase<TSequenceSetup> where TSequenceSetup : SequenceSetupBase
	{
		private readonly List<TSequenceSetup> sequenceSetupsInternal = new List<TSequenceSetup>();

		/// <summary>
		/// 
		/// </summary>
		protected IReadOnlyList<TSequenceSetup> SequenceSetups => sequenceSetupsInternal;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		public InvocationShapeSetupsBase(TSequenceSetup sequenceSetup)
		{
			sequenceSetupsInternal.Add(sequenceSetup);
			InvocationShape = sequenceSetup.SetupInternal.Expectation;
			sequenceSetup.InvocationShapeSetupsObject = this;
		}
		
		internal void AddSequenceSetup(TSequenceSetup sequenceSetup)
		{
			sequenceSetupsInternal.Add(sequenceSetup);
			sequenceSetup.InvocationShapeSetupsObject = this;
		}
		internal InvocationShape InvocationShape { get; }
	}

}
