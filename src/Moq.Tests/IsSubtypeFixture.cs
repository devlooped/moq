// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace Moq.Tests
{
	public class IsSubtypeFixture
	{
		[Fact]
		public void It_IsSubtype_works_for_base_type_relationships()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<ArgumentOutOfRangeException>(); // should match
			mock.Object.Method<Exception>();
			mock.Object.Method<ArgumentException>(); // should match
			mock.Object.Method<string>();
			mock.Object.Method<ArgumentNullException>(); // should match
			mock.Object.Method<InvalidOperationException>();
			mock.Object.Method<bool>();

			mock.Verify(m => m.Method<It.IsSubtype<ArgumentException>>(), Times.Exactly(3));
		}

		[Fact]
		public void It_IsSubtype_works_for_interface_implementation_relationships()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<Stream>(); // should match
			mock.Object.Method<string>();
			mock.Object.Method<IEnumerator<int>>(); // should match
			mock.Object.Method<Exception>();
			mock.Object.Method<bool>();

			mock.Verify(m => m.Method<It.IsSubtype<IDisposable>>(), Times.Exactly(2));
		}

		public interface IX
		{
			void Method<T>();
		}
	}
}
