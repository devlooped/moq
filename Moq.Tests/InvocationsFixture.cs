using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Moq.Tests
{
    public class InvocationsFixture
    {
	    [Fact]
	    public void MockInvocationsAreRecorded()
	    {
		    var mock = new Mock<IComparable>();

		    mock.Object.CompareTo(new object());

		    Assert.Equal(1, mock.Invocations.Count);
	    }

		[Fact]
	    public void MockInvocationsIncludeInvokedMethod()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(new object());

			var invocation = mock.Invocations[0];

			var expectedMethod = typeof(IComparable).GetMethod(nameof(mock.Object.CompareTo));

			Assert.Equal(expectedMethod, invocation.Method);
		}

	    [Fact]
	    public void MockInvocationsIncludeArguments()
	    {
			var mock = new Mock<IComparable>();

		    var obj = new object();

			mock.Object.CompareTo(obj);

		    var invocation = mock.Invocations[0];

		    var expectedArguments = new[] {obj};

			Assert.Equal(expectedArguments, invocation.Arguments);
	    }
	}
}
