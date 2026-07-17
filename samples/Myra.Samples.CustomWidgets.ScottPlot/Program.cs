namespace Myra.Samples.ScottPlot;

/// <summary>
/// Entry point for the CustomWidgets.ScottPlot sample.
/// Demonstrates integrating ScottPlot interactive charts with Myra's UI framework.
/// </summary>
class Program
{
	/// <summary>
	/// Main method - starts the ScottPlot integration game.
	/// </summary>
	static void Main(string[] args)
	{
		using (var game = new ScottPlotGame())
			game.Run();
	}
}