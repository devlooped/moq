using Microsoft.VisualStudio.DebuggerVisualizers;

namespace Moq.Visualizer
{
	public class MockVisualizer : DialogDebuggerVisualizer
	{
		protected override void Show(
			IDialogVisualizerService windowService,
			IVisualizerObjectProvider objectProvider)
		{
			using (var visualizer = new MockVisualizerForm(objectProvider.GetObject()))
			{
				windowService.ShowDialog(visualizer);
			}
		}
	}
}