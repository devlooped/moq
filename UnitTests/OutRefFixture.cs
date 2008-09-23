using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq.Language.Flow;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Tests
{
	public class OutRefFixture
	{
		[Fact(Skip = "Not implemented yet")]
		public void ExpectsOutArgument()
		{
			var mock = new Mock<IFoo>();
			string expected = "ack";

			mock.Expect(m => m.Execute("ping", out expected)).Returns(true);

			string actual;
			bool ok = mock.Object.Execute("ping", out actual);

			Assert.True(ok);
			Assert.Equal(expected, actual);
		}

		public interface IFoo
		{
			bool Execute(string command, out string result);
			int Value { get; set; }
		}
	}
}
