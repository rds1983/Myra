namespace Myra.Samples;

/// <summary>
/// Entry point for the LogView sample. Creates and runs the <see cref="LogViewGame"/>.
/// </summary>
class Program
{
	static void Main(string[] args)
	{
		using (var game = new LogViewGame())
			game.Run();
	}
}
