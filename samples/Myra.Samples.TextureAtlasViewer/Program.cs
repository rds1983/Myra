using AssetManagementBase;
using System;

namespace Myra.Samples;

/// <summary>
/// The main class.
/// </summary>
class Program
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	/// <param name="args">The command line arguments. The first argument must be the path to a texture atlas (<c>.xmat</c>) file.</param>
	[STAThread]
	static void Main(string[] args)
	{
		try
		{
			AMBConfiguration.Logger = Console.WriteLine;

			if (args.Length == 0)
			{
				Console.WriteLine("Usage: Myra.Samples.TextureAtlasViewer.exe <texture_atlas.xmat>");
				return;
			}

			var path = args[0];
			using (var game = new TextureAtlasViewerGame(path))
				game.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}
