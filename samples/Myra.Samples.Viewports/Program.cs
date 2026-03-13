namespace Myra.Samples.AllWidgets
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new AllWidgetsGame())
				game.Run();
		}
	}
}
