using System;

namespace Myra.Samples.AllWidgets
{
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var test = new AllWidgetsGame();
				test.Run();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
