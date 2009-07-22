namespace Moq.Visualizer
{
	partial class MockVisualizerForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.visualizerHost = new System.Windows.Forms.Integration.ElementHost();
			this.mockVisualizerView = new Moq.Visualizer.MockVisualizerView();
			this.SuspendLayout();
			// 
			// visualizerHost
			// 
			this.visualizerHost.CausesValidation = false;
			this.visualizerHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.visualizerHost.Location = new System.Drawing.Point(0, 0);
			this.visualizerHost.Name = "visualizerHost";
			this.visualizerHost.Size = new System.Drawing.Size(453, 305);
			this.visualizerHost.TabIndex = 0;
			this.visualizerHost.Child = this.mockVisualizerView;
			// 
			// MockVisualizerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(453, 305);
			this.Controls.Add(this.visualizerHost);
			this.MinimizeBox = false;
			this.Name = "MockVisualizerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mock Debugger Visualizer";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost visualizerHost;
		private MockVisualizerView mockVisualizerView;



	}
}