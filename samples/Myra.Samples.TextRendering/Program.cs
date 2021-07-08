namespace Myra.Samples.TextRendering
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new TextRenderingGame())
				game.Run();
		}
	}
}
