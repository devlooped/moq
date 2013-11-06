using System;
using System.Diagnostics.CodeAnalysis;

namespace Moq.Language
{
	/// <summary>
	/// Language for ReturnSequence
	/// </summary>
	public interface ISetupSequentialResult<TResult>
	{
		// would be nice to Mixin somehow the IReturn and IThrows with
		// another ReturnType

		/// <summary>
		/// Returns value
		/// </summary>
		ISetupSequentialResult<TResult> Returns(TResult value);

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialResult<TResult> Throws(Exception exception);

		/// <summary>
		/// Throws an exception
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By Design")]
		ISetupSequentialResult<TResult> Throws<TException>() where TException : Exception, new();

		/// <summary>
		/// Calls original method
		/// </summary>
		ISetupSequentialResult<TResult> CallBase();
	}
}