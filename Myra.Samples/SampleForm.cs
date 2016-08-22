using System;
using System.Windows.Forms;

namespace Myra.Samples
{
	public delegate void SampleLauncher(Type sampleType);

	public partial class SampleForm : Form
	{
		public SampleLauncher Launcher;

		public SampleForm()
		{
			InitializeComponent();

			foreach (var s in SampleGame.AllSampleTypes)
			{
				_listBoxSamples.Items.Add(s.Name);
			}

			_listBoxSamples.SelectedIndex = 3;

			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			_buttonRun.Enabled = _listBoxSamples.SelectedIndex != -1;
		}

		private void _listBoxSamples_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void _buttonRun_Click(object sender, EventArgs e)
		{
			var sample = SampleGame.AllSampleTypes[_listBoxSamples.SelectedIndex];

			if (Launcher != null)
			{
				Launcher(sample);
			}
		}
	}
}
