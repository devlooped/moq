using Microsoft.VisualStudio.DebuggerVisualizers;

namespace Moq.Visualizer
{
	public class MockVisualizer : DialogDebuggerVisualizer
	{
		protected override void Show(
			IDialogVisualizerService windowService,
			IVisualizerObjectProvider objectProvider)
		{
			windowService.ShowDialog(new MockVisualizerForm(objectProvider.GetObject()));
		}
	}
}