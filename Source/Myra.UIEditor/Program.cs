using System;
using log4net.Config;

namespace Myra.UIEditor
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
				XmlConfigurator.Configure();			
				using (var studio = new Studio())
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
