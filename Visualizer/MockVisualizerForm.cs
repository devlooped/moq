using System.Windows.Forms;

namespace Moq.Visualizer
{
	public partial class MockVisualizerForm : Form
	{
		public MockVisualizerForm(object context)
		{
			this.InitializeComponent();
			this.visualizerHost.HostContainer.DataContext = context;
		}
	}
}