using System.Windows.Forms;
using Moq.Visualizer.ViewModel;

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