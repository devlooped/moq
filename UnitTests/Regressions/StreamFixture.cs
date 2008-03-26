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
		}
	}

	public class MyStream : Stream
	{
		public override bool CanRead
		{
			get { throw new NotImplementedException(); }
		}

		public override bool CanSeek
		{
			get { throw new NotImplementedException(); }
		}

		public override bool CanWrite
		{
			get { throw new NotImplementedException(); }
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 10;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
