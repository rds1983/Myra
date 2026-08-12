namespace Myra.Samples.CustomWidgets
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new ArrowGame())
				game.Run();
		}
	}
}
