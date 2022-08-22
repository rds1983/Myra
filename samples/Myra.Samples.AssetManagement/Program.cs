using System;

namespace Myra.Samples.AssetManagement
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
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
