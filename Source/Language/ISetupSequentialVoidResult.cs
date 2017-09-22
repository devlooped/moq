using System;

namespace Moq.Language
{
	/// <summary>
	/// Language for ReturnSequence
	/// </summary>
	public interface ISetupSequentialVoidResult
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		ISetupSequentialVoidResult Pass();

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialVoidResult Throws<TException>() where TException : Exception, new();

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialVoidResult Throws(Exception exception);
	}
}
