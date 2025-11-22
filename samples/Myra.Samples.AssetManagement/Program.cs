using AssetManagementBase;
using System;

namespace Myra.Samples.AssetManagement
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				AMBConfiguration.Logger = Console.WriteLine;
				using (var game = new AssetManagementGame())
					game.Run();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
