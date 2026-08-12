using AssetManagementBase;
using System;

namespace Myra.Samples.SkiaSharp;

/// <summary>
/// Entry point for the CustomWidgets.SkiaSharp sample.
/// Demonstrates integrating SkiaSharp vector graphics with Myra's UI framework.
/// </summary>
class Program
{
	/// <summary>
	/// Main method - configures logging and starts the SkiaSharp integration game.
	/// </summary>
	static void Main(string[] args)
	{
		try
		{
			// Enable console logging for asset management operations
			AMBConfiguration.Logger = Console.WriteLine;

			// Create and run the main game window with SkiaSharp rendering
			using (var game = new SkiaSharpGame())
				game.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
}
