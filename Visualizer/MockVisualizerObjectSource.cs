using System.IO;
using System.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Moq.Proxy;

namespace Moq.Visualizer
{
	public class MockVisualizerObjectSource : VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			VisualizerObjectSource.Serialize(outgoingData, new MockContextViewModel((Mock)target));
		}
	}
}