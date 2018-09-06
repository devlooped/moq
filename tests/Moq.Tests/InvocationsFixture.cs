// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
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

	    [Fact]
	    public void MockInvocationsCanBeEnumerated()
	    {
			var mock = new Mock<IComparable>();

		    mock.Object.CompareTo(-1);
		    mock.Object.CompareTo(0);
		    mock.Object.CompareTo(1);

		    var count = 0;

		    using (var enumerator = mock.Invocations.GetEnumerator())
		    {
			    while (enumerator.MoveNext())
			    {
				    Assert.NotNull(enumerator.Current);

				    count++;
			    }
		    }

			Assert.Equal(3, count);
	    }

	    [Fact]
	    public void MockInvocationsCanBeCleared()
	    {
		    var mock = new Mock<IComparable>();

		    mock.Object.CompareTo(new object());

			mock.Invocations.Clear();

		    Assert.Equal(0, mock.Invocations.Count);
	    }

		[Fact]
	    public void MockInvocationsCanBeRetrievedByIndex()
	    {
			var mock = new Mock<IComparable>();

		    mock.Object.CompareTo(-1);
		    mock.Object.CompareTo(0);
		    mock.Object.CompareTo(1);

		    var invocation = mock.Invocations[1];

		    var arg = invocation.Arguments[0];

			Assert.Equal(0, arg);
	    }

		[Fact]
	    public void MockInvocationsIndexerThrowsIndexOutOfRangeWhenCollectionIsEmpty()
	    {
			var mock = new Mock<IComparable>();

			Assert.Throws<IndexOutOfRangeException>(() => mock.Invocations[0]);
	    }
	}
}
