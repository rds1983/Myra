using System;

namespace Myra.Samples.FantasyMapGenerator
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (var game = new FantasyMapGeneratorGame())
				game.Run();
		}
	}
}