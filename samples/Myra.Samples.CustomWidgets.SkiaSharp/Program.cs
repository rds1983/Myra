using AssetManagementBase;
using System;

namespace Myra.Samples.SkiaSharp;

class Program
{
	static void Main(string[] args)
	{
		try
		{
			AMBConfiguration.Logger = Console.WriteLine;
			using (var game = new SkiaSharpGame())
				game.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
}
