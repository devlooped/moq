using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq
{
	/// <summary>
	/// Makes legacy members on <see cref="MockFactory"/> visible.
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
			factory.Verify();
		}

		/// <summary>
		/// Verifies all verifiable expectations on all mocks created 
		/// by this factory.
		/// </summary>
		/// <exception cref="MockException">One or more mocks had expectations that were not satisfied.</exception>
		public static void VerifyAll(this MockFactory factory)
		{
			factory.VerifyAll();
		}
	}
}
