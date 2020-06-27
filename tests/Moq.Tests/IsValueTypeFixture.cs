// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class IsValueTypeFixture
	{
		[Fact]
		public void It_IsValueType_works_for_unconstrained_type_parameter()
		{
			var mock = new Mock<IX>();

			mock.Object.Unconstrained(1); // should match
			mock.Object.Unconstrained(true); // should match
			mock.Object.Unconstrained("");
			mock.Object.Unconstrained(new Exception());
			mock.Object.Unconstrained(3.141f); // should match

			mock.Verify(m => m.Unconstrained<It.IsValueType>(It.IsAny<It.IsValueType>()), Times.Exactly(3));
		}

		[Fact]
		public void It_IsValueType_works_for_constrained_type_parameter()
		{
			var mock = new Mock<IX>();

			mock.Object.Constrained(1); // should match
			mock.Object.Constrained(true); // should match
			//mock.Object.Constrained("");
			//mock.Object.Constrained(new Exception());
			mock.Object.Constrained(3.141f); // should match

			mock.Verify(m => m.Constrained<It.IsValueType>(It.IsAny<It.IsValueType>()), Times.Exactly(3));
		}

		public interface IX
		{
			void Unconstrained<T>(T arg);
			void Constrained<T>(T arg) where T : struct;
		}
	}
}
