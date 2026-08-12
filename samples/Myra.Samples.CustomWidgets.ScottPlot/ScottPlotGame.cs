using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Linq;

namespace Myra.Samples.ScottPlot;

public class ScottPlotGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private Desktop _desktop;
	private MyraPlot _myraPlot;

	public ScottPlotGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800
		};
		Window.AllowUserResizing = true;
		IsMouseVisible = true;
	}

	protected override void LoadContent()
	{
		base.LoadContent();

		MyraEnvironment.Game = this;

		_myraPlot = new MyraPlot();
		PlotPrices();

		var btnPrices = new Button
		{
			Width = 100,
			Content = new Graphics2D.UI.Label
			{
				HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Center,
				Text = "Prices"
			}
		};
		btnPrices.Click += (s, e) => PlotPrices();

		var btnScatter = new Button
		{
			Width = 100,
			Content = new Graphics2D.UI.Label
			{
				HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Center,
				Text = "Scatter"
			}
		};
		btnScatter.Click += (s, e) => PlotScatter();

		var btnBar = new Button
		{
			Width = 100,
			Content = new Graphics2D.UI.Label
			{
				HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Center,
				Text = "Bars"
			}
		};
		btnBar.Click += (s, e) => PlotBars();

		var buttonPanel = new VerticalStackPanel
		{
			Widgets =
			{
				btnPrices,
				btnScatter,
				btnBar
			},
			HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Center,
			Spacing = 8
		};

		var topPanel = new HorizontalSplitPane();

		topPanel.Widgets.Add(_myraPlot);
		topPanel.Widgets.Add(buttonPanel);

		topPanel.SetSplitterPosition(0, 0.75f);

		_desktop = new Desktop
		{
			Root = topPanel
		};
	}

	private void PlotPrices()
	{
		_myraPlot.Reset();

		var rand = new Random(42);
		int days = 365;
		DateTime start = new DateTime(2025, 1, 1);

		string[] names = { "ACME", "GLOBX", "ZNTH", "KYTE" };
		double[] startingPrices = { 120, 45, 200, 75 };

		for (int s = 0; s < names.Length; s++)
		{
			DateTime[] dates = Enumerable.Range(0, days).Select(i => start.AddDays(i)).ToArray();
			double[] prices = new double[days];
			prices[0] = startingPrices[s];

			for (int i = 1; i < days; i++)
			{
				double change = (rand.NextDouble() - 0.48) * startingPrices[s] * 0.02;
				prices[i] = prices[i - 1] + change;
			}

			var sig = _myraPlot.Plot.Add.SignalXY(dates.Select(d => d.ToOADate()).ToArray(), prices);
			sig.LegendText = names[s];
		}

		_myraPlot.Plot.Title("Stock Prices");
		_myraPlot.Plot.Legend.IsVisible = true;
		_myraPlot.Plot.Axes.DateTimeTicksBottom();
	}

	private void PlotScatter()
	{
		_myraPlot.Reset();
		var rand = new Random(0);
		double[] xs = Enumerable.Range(0, 50).Select(_ => rand.NextDouble() * 100).ToArray();
		double[] ys = Enumerable.Range(0, 50).Select(_ => rand.NextDouble() * 100).ToArray();
		_myraPlot.Plot.Add.Scatter(xs, ys);
		_myraPlot.Plot.Title("Random Scatter");
	}

	private void PlotBars()
	{
		_myraPlot.Reset();
		double[] values = { 5, 11, 3, 8, 14, 7, 10, 12, 6, 9 };
		_myraPlot.Plot.Add.Bars(values);
		_myraPlot.Plot.Title("Bar Chart");
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);

		GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
		_desktop.Render();
	}
}