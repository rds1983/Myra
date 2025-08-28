global using Myra.Events;
using AssetManagementBase;
using System;

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
				AMBConfiguration.Logger = Console.WriteLine;
				using (var studio = new Studio(args))
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
