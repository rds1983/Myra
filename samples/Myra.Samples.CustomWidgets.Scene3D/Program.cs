namespace Myra.Samples.CustomWidgets
{
	/// <summary>
	/// Entry point for the Scene3D sample. Creates and runs the <see cref="Scene3DGame"/>.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new Scene3DGame())
				game.Run();
		}
	}
}
