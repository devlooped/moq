using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;

namespace Moq.Tests
{
	public class StreamFixture
	{
		[Fact]
		public void ShouldMockStream()
		{
			var mockStream = new Mock<Stream>();

			mockStream.Expect(stream => stream.Seek(0, SeekOrigin.Begin)).Returns(0L);

			var position = mockStream.Object.Seek(0, SeekOrigin.Begin);

			Assert.Equal(0, position);

			mockStream.Expect(stream => stream.Flush());
			mockStream.Expect(stream => stream.SetLength(100));

			mockStream.Object.Flush();
			mockStream.Object.SetLength(100);

			mockStream.VerifyAll();
		}
	}
}
