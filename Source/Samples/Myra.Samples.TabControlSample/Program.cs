using Myra.Samples.TabControlSample;

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
