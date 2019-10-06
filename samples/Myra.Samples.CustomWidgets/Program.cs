namespace Myra.Samples.AllWidgets
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new CustomWidgetsGame())
				game.Run();
		}
	}
}
