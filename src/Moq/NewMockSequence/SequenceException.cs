using System;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public partial class SequenceException : Exception
	{
		internal SequenceException(Times times, int executedCount, ISetup setup) : 
			base($"{times.GetExceptionMessage(executedCount)}{(setup == null ? "" : $"{setup}")}") { }
	}

}
