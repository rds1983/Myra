namespace Myra.Samples.AllWidgetsSample
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
