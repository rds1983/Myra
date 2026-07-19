namespace Myra.Samples;

class Program
{
	static void Main(string[] args)
	{
		using (var game = new DataGridGame())
			game.Run();
	}
}
