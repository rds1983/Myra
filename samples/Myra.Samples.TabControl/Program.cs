using Myra.Samples.TabControl;

namespace Myra.Samples.AllWidgetsSample
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new TabControlGame())
				game.Run();
		}
	}
}
