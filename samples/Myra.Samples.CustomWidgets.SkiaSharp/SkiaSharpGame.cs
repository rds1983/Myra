using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using SkiaSharp;

namespace Myra.Samples.SkiaSharp;

/// <summary>
/// Main game class demonstrating SkiaSharp integration with Myra.
/// Creates a split-pane layout with a SkiaSharp-rendered canvas and a text input,
/// showing how third-party graphics libraries can be seamlessly integrated.
/// </summary>
public class SkiaSharpGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private Desktop _desktop;
	private TextBox _textBox;

	/// <summary>
	/// Initializes the game window with desired resolution and input settings.
	/// </summary>
	public SkiaSharpGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800
		};
		Window.AllowUserResizing = true;
		IsMouseVisible = true;
	}

	/// <summary>
	/// Loads content and sets up the Myra UI with a SkiaSharp canvas and text input.
	/// </summary>
	protected override void LoadContent()
	{
		base.LoadContent();

		// Initialize Myra environment with the current game instance
		MyraEnvironment.Game = this;

		// Create a SkiaSharp canvas widget with custom drawing callback
		var canvas = new SKCanvasWidget
		{
			PaintHandler = PaintHandler
		};

		// Create a text input box for user interaction
		_textBox = new TextBox
		{
			Text = "Hello, Myra!",
			Width = 200,
			Height = 30
		};

		// Create a horizontal split pane to display both widgets side-by-side
		var topPanel = new HorizontalSplitPane();

		topPanel.Widgets.Add(canvas);
		topPanel.Widgets.Add(_textBox);

		// Set the canvas to occupy 75% of the horizontal space
		topPanel.SetSplitterPosition(0, 0.75f);

		// Create the Myra desktop with the split pane as root widget
		_desktop = new Desktop
		{
			Root = topPanel
		};
	}

	/// <summary>
	/// Custom SkiaSharp drawing handler that renders graphics to the canvas.
	/// Draws a blue background, red circle, and text from the text input box.
	/// </summary>
	private void PaintHandler(Point size, SKCanvas canvas)
	{
		// Clear the canvas with a blue background
		canvas.Clear(SKColors.CornflowerBlue);

		// Draw a red circle in the center of the canvas
		using (var paint = new SKPaint
		{
			Color = SKColors.Red,
			IsAntialias = true,
			Style = SKPaintStyle.Fill
		})
		{
			canvas.DrawCircle(size.X / 2f, size.Y / 2f, 100, paint);
		}

		// Draw text from the text box input at the top of the canvas
		using (var font = new SKFont { Size = 64.0f })
		using (var paint = new SKPaint())
		{
			paint.IsAntialias = true;
			paint.Color = new SKColor(0x42, 0x81, 0xA4);
			paint.IsStroke = false;

			canvas.DrawText(_textBox.Text, size.X / 2f, 64.0f, SKTextAlign.Left, font, paint);
		}

		// Flush all pending drawing operations to the surface
		canvas.Flush();
	}

	/// <summary>
	/// Renders the game frame by clearing the screen and drawing the Myra UI.
	/// </summary>
	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);

		GraphicsDevice.Clear(Color.Black);
		_desktop.Render();
	}
}