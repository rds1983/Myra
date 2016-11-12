using System;
using Microsoft.Xna.Framework;

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
			MyraEnvironment.IsWindowsDX = true;
			Game sample;
			using (var form = new SampleForm())
			{
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
