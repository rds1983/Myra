namespace Myra.Samples.ObjectEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new DebugConsoleGame())
				game.Run();
		}
	}
}