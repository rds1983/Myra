using System;

namespace MyraPad
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				using (var studio = new Studio())
				{
					studio.Run();
				}
			}
			catch (Exception ex)
			{
#if !CORE
				MessageBox.Show(ex.ToString());
#endif			
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
