using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using NvgSharp;

namespace Myra.Samples;

/// <summary>
/// Sample game demonstrating integration of NvgSharp vector graphics with Myra.
/// Creates a <see cref="NvgCanvasWidget"/> that renders a NanoVG demo scene inside
/// a Myra UI layout, with interactive controls for toggling rendering options.
/// </summary>
public class NvgSharpGame : Game
{
	private readonly GraphicsDeviceManager _graphics;

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
			// Depth24Stencil8 is required by NvgSharp for stencil-based stroke rendering
			PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
		};

		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		Window.AllowUserResizing = true;
		IsFixedTimeStep = false;
		_graphics.SynchronizeWithVerticalRetrace = false;
	}

	/// <summary>
	/// Builds the Myra UI layout and initialises the NvgSharp demo resources.
	/// The layout consists of a checkbox row for toggling edge antialiasing,
	/// a separator, and the NvgSharp canvas filling the remaining space.
	/// </summary>
	protected override void LoadContent()
	{
		MyraEnvironment.Game = this;

		// Create the NvgSharp canvas widget and assign the draw callback
		_nvgCanvasWidget = new NvgCanvasWidget
		{
			PaintHandler = PaintHandler
		};

		// Build a small controls panel at the top
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

		// Stack: controls panel, separator, canvas (fill)
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

	/// <summary>
	/// Rendering callback passed to <see cref="NvgCanvasWidget.PaintHandler"/>.
	/// Draws the NanoVG demo scene and the frame-time performance graph.
	/// </summary>
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
	/// Clears the screen and renders the Myra desktop, which in turn triggers
	/// <see cref="NvgCanvasWidget.InternalRender"/> for the canvas widget.
	/// </summary>
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(0.1f, 0.1f, 0.1f));

		_desktop.Render();
	}
}
