using System;
using Microsoft.Xna.Framework;
using Myra.Samples.Graphics3D;
using Myra.Samples.WinForms;

namespace Myra.Samples.WindowsDXLauncher
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Game sample;
			using (var form = new SampleForm())
			{
				foreach (var t in Samples2D.AllSampleTypes)
				{
					form.AddSampleType(t);
				}

				foreach (var t in Samples3D.AllSampleTypes)
				{
					form.AddSampleType(t);
				}

				form.Launcher += sampleType =>
				{
					using (sample = (Game)Activator.CreateInstance(sampleType))
					{
						sample.Window.AllowUserResizing = true;
						sample.Run();
					}
				};

				form.ShowDialog();
			}
		}
	}
}
