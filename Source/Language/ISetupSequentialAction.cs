using System;

namespace Moq.Language
{
	/// <summary>
	/// Language for ReturnSequence
	/// </summary>
	public interface ISetupSequentialAction
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		ISetupSequentialAction Pass();

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialAction Throws<TException>() where TException : Exception, new();

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialAction Throws(Exception exception);
	}
}
