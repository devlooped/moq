using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Linq.Expressions;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;

namespace Moq.Tests
{
	[TestFixture]
	public class StreamFixture
	{
		[Test]
		public void ShouldMockStream()
		{
			var mockStream = new Mock<Stream>();

			mockStream.Expect(stream => stream.Seek(0, SeekOrigin.Begin)).Returns(0L);

			var position = mockStream.Object.Seek(0, SeekOrigin.Begin);

			Assert.AreEqual(0, position);

			mockStream.Expect(stream => stream.Flush());
			mockStream.Expect(stream => stream.SetLength(100));

			mockStream.Object.Flush();
			mockStream.Object.SetLength(100);

			mockStream.VerifyAll();
		}
	}
}
