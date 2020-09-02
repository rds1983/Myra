using System;

#if !CORE
using System.Windows.Forms;
#endif

namespace MyraPad
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				using (var studio = new Studio(args))
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
