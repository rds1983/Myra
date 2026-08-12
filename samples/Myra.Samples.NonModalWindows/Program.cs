global using Myra.Events;

namespace Myra.Samples.NonModalWindows
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new NonModalWindowsGame())
				game.Run();
		}
	}
}
