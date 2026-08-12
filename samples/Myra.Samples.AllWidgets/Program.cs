using AssetManagementBase;
using System;

namespace Myra.Samples.AllWidgets
{
	class Program
	{
		static void Main(string[] args)
		{
			AMBConfiguration.Logger = Console.WriteLine;
			using (var game = new AllWidgetsGame())
				game.Run();
		}
	}
}
