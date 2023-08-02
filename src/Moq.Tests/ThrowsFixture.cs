// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

using Xunit;

namespace Moq.Tests
{
	public class ThrowsFixture
	{
		[Fact]
		public void PassesOneArgumentToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>()))
				.Throws((string s) => new Exception(s));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1"));
			Assert.Equal("blah1", exception.Message);
		}

		[Fact]
		public void PassesTwoArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2) => new Exception(s1 + s2));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2"));
			Assert.Equal("blah1blah2", exception.Message);
		}

		[Fact]
		public void PassesThreeArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3) => new Exception(s1 + s2 + s3));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3"));
			Assert.Equal("blah1blah2blah3", exception.Message);
		}

		[Fact]
		public void PassesFourArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3, string s4) => new Exception(s1 + s2 + s3 + s4));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3", "blah4"));
			Assert.Equal("blah1blah2blah3blah4", exception.Message);
		}

		[Fact]
		public void PassesFiveArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3, string s4, string s5) => new Exception(s1 + s2 + s3 + s4 + s5));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5"));
			Assert.Equal("blah1blah2blah3blah4blah5", exception.Message);
		}

		[Fact]
		public void PassesSixArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3, string s4, string s5, string s6) => new Exception(s1 + s2 + s3 + s4 + s5 + s6));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6"));
			Assert.Equal("blah1blah2blah3blah4blah5blah6", exception.Message);
		}

		[Fact]
		public void PassesSevenArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3, string s4, string s5, string s6, string s7) => new Exception(s1 + s2 + s3 + s4 + s5 + s6 + s7));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7"));
			Assert.Equal("blah1blah2blah3blah4blah5blah6blah7", exception.Message);
		}

		[Fact]
		public void PassesEightArgumentsToThrows()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws((string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) => new Exception(s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8));

			var exception = Assert.Throws<Exception>(() => mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7", "blah8"));
			Assert.Equal("blah1blah2blah3blah4blah5blah6blah7blah8", exception.Message);
		}

		public interface IFoo
		{
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8);
		}
	}
}
