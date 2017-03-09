using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Myra.Samples.WinForms
{
	public delegate void SampleLauncher(Type sampleType);

	public partial class SampleForm : Form
	{
		private readonly List<Type> _sampleTypes = new List<Type>();

		public SampleLauncher Launcher;

		public SampleForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			foreach (var s in _sampleTypes)
			{
				_listBoxSamples.Items.Add(s.Name);
			}

			_listBoxSamples.SelectedIndex = 3;

			UpdateEnabled();
		}

		public void AddSampleType(Type type)
		{
			if (_sampleTypes.Contains(type))
			{
				return;
			}

			_sampleTypes.Add(type);
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
			_listBoxSamples.Enabled = false;

			var sample = _sampleTypes[_listBoxSamples.SelectedIndex];

			if (Launcher != null)
			{
				Launcher(sample);
			}

			_listBoxSamples.Enabled = true;
		}
	}
}
