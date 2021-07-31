// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SequenceSetupBase
	{
		/// <summary>
		/// 
		/// </summary>
		protected internal object InvocationShapeSetupsObject { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int SetupIndex { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ISetup Setup => SetupInternal;
		internal Setup SetupInternal { get; set; }

	}

}
