using System;
using System.Windows.Forms;

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
				MessageBox.Show(ex.ToString());
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
