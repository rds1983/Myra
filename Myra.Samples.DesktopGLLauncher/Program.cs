using System;

namespace Myra.Samples.DesktopGLLauncher
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			SampleGame sample;
			using (var form = new SampleForm())
			{
				form.Launcher += sampleType =>
				{
					using (sample = (SampleGame) Activator.CreateInstance(sampleType))
					{
						sample.Run();
					}
				};

				form.ShowDialog();
			}
		}
	}
}
