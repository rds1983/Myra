using System;
using System.Windows.Forms;

namespace Myra.UIEditor
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
				if (args.Length < 1)
				{
					Console.WriteLine("Usage: Myra.UIViewer <project.xml>");
					return;
				}

				using (var studio = new Viewer(args[0]))
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
