using System;
using Microsoft.Xna.Framework;

namespace Myra.Samples.FNALauncher
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
				form.Launcher += sampleType =>
				{
					var result = Activator.CreateInstance(sampleType);
					using (sample = (Game)result)
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
