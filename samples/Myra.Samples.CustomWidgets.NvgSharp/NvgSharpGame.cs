using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using NvgSharp;

namespace Myra.Samples;

/// <summary>
/// This is the main type for your game.
/// </summary>
public class NvgSharpGame : Game
{
	GraphicsDeviceManager _graphics;

	private Demo _demo;
	private PerfGraph _perfGraph;
	private NvgCanvasWidget _nvgCanvasWidget;
	private Desktop _desktop;
	private GameTime _gameTime;

	public NvgSharpGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800,
			PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
		};

		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		Window.AllowUserResizing = true;
		IsFixedTimeStep = false;
		_graphics.SynchronizeWithVerticalRetrace = false;
	}

	/// <summary>
	/// LoadContent will be called once per game and is the place to load
	/// all of your content.
	/// </summary>
	protected override void LoadContent()
	{
		MyraEnvironment.Game = this;

		_nvgCanvasWidget = new NvgCanvasWidget
		{
			PaintHandler = PaintHandler
		};

		var buttonsPanel = new HorizontalStackPanel
		{
			Spacing = 8
		};

		var checkEdgeAntialiasing = new CheckButton
		{
			Content = new Label
			{
				Text = "Edge Antialiasing"
			},
			IsChecked = _nvgCanvasWidget.EdgeAntialiasing
		};
		checkEdgeAntialiasing.IsCheckedChanged += (s, a) => _nvgCanvasWidget.EdgeAntialiasing = checkEdgeAntialiasing.IsChecked;
		buttonsPanel.Widgets.Add(checkEdgeAntialiasing);

		var topPanel = new VerticalStackPanel();
		topPanel.Widgets.Add(buttonsPanel);
		topPanel.Widgets.Add(new HorizontalSeparator());

		StackPanel.SetProportionType(_nvgCanvasWidget, ProportionType.Fill);
		topPanel.Widgets.Add(_nvgCanvasWidget);

		_desktop = new Desktop
		{
			Root = topPanel
		};

		_demo = new Demo();
		_perfGraph = new PerfGraph(PerfGraph.Style.GRAPH_RENDER_FPS, "Frame Time", _demo.fontSystemNormal);
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		_perfGraph.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

		_gameTime = gameTime;
	}

	private void PaintHandler(NvgContext nvgContext, Point size)
	{
		var t = (float)_gameTime.TotalGameTime.TotalSeconds;

		var vp = GraphicsDevice.Viewport;

		var mouseState = Mouse.GetState();
		_demo.renderDemo(nvgContext,
			mouseState.X,
			mouseState.Y,
			vp.Width,
			vp.Height,
			t,
			false);

		_perfGraph.Render(nvgContext, 5, 5);

	}

	/// <summary>
	/// This is called when the game should draw itself.
	/// </summary>
	/// <param name="gameTime">Provides a snapshot of timing values.</param>
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(0.1f, 0.1f, 0.1f));

		// TODO: Add your drawing code here
		_desktop.Render();
	}
}