using System;
using NLog;

namespace Myra.UIEditor
{
	static class Program
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
				_logger.Error(ex);
			}
		}
	}
}
