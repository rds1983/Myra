namespace Myra.Samples.AssetManagement
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new AssetManagementGame())
				game.Run();
		}
	}
}
