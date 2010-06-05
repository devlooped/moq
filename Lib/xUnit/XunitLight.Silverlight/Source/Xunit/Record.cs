namespace Xunit
{
	using Xunit.Sdk;
	using System;

	public class Record
	{
		/// <summary>
		/// Records any exception which is thrown by the given code.
		/// </summary>
		/// <param name="code">The code which may thrown an exception.</param>
		/// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
		public static Exception Exception(Assert.ThrowsDelegate code)
		{
			try
			{
				code();
				return null;
			}
			catch (Exception ex)
			{
				return ex;
			}
		}
	}
}
