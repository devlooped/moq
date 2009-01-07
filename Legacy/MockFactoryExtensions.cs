using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq
{
	/// <summary>
	/// Provides extensions for legacy code for <see cref="MockFactory"/>.
	/// </summary>
	public static class MockFactoryExtensions
	{
		/// <summary>
		/// Verifies all verifiable expectations on all mocks created 
		/// by this factory.
		/// </summary>
		/// <exception cref="MockException">One or more mocks had expectations that were not satisfied.</exception>
		public static void Verify(this MockFactory factory)
		{
			factory.VerifyMocks(verifiable => verifiable.Verify());
		}

		/// <summary>
		/// Verifies all verifiable expectations on all mocks created 
		/// by this factory.
		/// </summary>
		/// <exception cref="MockException">One or more mocks had expectations that were not satisfied.</exception>
		public static void VerifyAll(this MockFactory factory)
		{
			factory.VerifyMocks(verifiable => verifiable.VerifyAll());
		}

		/// <summary>
		/// Invokes <paramref name="verifyAction"/> for each mock 
		/// in <see cref="MockFactory.Mocks"/>, and accumulates the resulting 
		/// <see cref="MockVerificationException"/> that might be 
		/// thrown from the action.
		/// </summary>
		/// <param name="verifyAction">The action to execute against 
		/// each mock.</param>
		private static void VerifyMocks(this MockFactory factory, Action<Mock> verifyAction)
		{
			StringBuilder message = new StringBuilder();

			foreach (var mock in factory.Mocks)
			{
				try
				{
					verifyAction(mock);
				}
				catch (MockVerificationException mve)
				{
					message.AppendLine(mve.GetRawSetups());
				}
			}

			if (message.ToString().Length > 0)
				throw new MockException(MockException.ExceptionReason.VerificationFailed,
					String.Format(Properties.Resources.VerficationFailed, message));
		}
	}
}
