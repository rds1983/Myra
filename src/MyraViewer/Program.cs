global using Myra.Events;
using AssetManagementBase;
using System;

namespace MyraViewer
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
				if (args.Length == 0)
				{
					Console.WriteLine("Usage: myraviewer <file.xmmp>");
					return;
				}

				AMBConfiguration.Logger = Console.WriteLine;
				using (var studio = new ViewerGame(args[0]))
				{
					studio.Run();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
