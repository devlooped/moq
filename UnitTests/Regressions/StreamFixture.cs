using System.IO;
using Xunit;

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
