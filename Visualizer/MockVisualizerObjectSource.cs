using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;

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