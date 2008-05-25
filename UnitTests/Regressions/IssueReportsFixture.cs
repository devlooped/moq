using System.Diagnostics;
using Xunit;

namespace Moq.Tests.Regressions
{
	public class IssueReportsFixture
	{
		[Fact]
		public void Repro()
		{
			Mock<TraceListener> traceListener = new Mock<TraceListener>();
			traceListener.Expect(x => x.TraceEvent(It.IsAny<TraceEventCache>(),
  It.IsAny<string>(), It.IsAny<TraceEventType>(), It.IsAny<int>(),
  It.IsAny<string>())).Verifiable();

			traceListener.Object.TraceEvent(null, "", TraceEventType.Verbose, 0, "");
			traceListener.Verify();
		}
	}
}
