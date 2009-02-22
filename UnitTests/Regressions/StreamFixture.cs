using System.IO;
using Xunit;
using System.Diagnostics;

namespace Moq.Tests
{
	public class StreamFixture
	{
#if !SILVERLIGHT
		[Conditional("DESKTOP")]
		[Fact]
		public void ShouldMockStream()
		{
			var mockStream = new Mock<Stream>();

			mockStream.Setup(stream => stream.Seek(0, SeekOrigin.Begin)).Returns(0L);

			var position = mockStream.Object.Seek(0, SeekOrigin.Begin);

			Assert.Equal(0, position);

			mockStream.Setup(stream => stream.Flush());
			mockStream.Setup(stream => stream.SetLength(100));

			mockStream.Object.Flush();
			mockStream.Object.SetLength(100);

			mockStream.VerifyAll();
		}
#endif
	}
}
