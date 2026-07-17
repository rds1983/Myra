using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Properties;
using Myra.SkiaSharp;
using SkiaSharp;

namespace Myra.Samples.SkiaSharp
{
	public class SkiaSharpGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private Desktop _desktop;
		private PropertyGrid _propertyGrid;

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

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			var canvas = new SKCanvasWidget();
			canvas.PaintHandler = PaintHandler;

			var scrollViewer = new ScrollViewer
			{
				ShowHorizontalScrollBar = false
			};

			_propertyGrid = new PropertyGrid
			{
				Object = canvas
			};

			scrollViewer.Content = _propertyGrid;

			var topPanel = new HorizontalSplitPane();

			topPanel.Widgets.Add(canvas);
			topPanel.Widgets.Add(scrollViewer);

			topPanel.SetSplitterPosition(0, 0.75f);

			_desktop = new Desktop
			{
				Root = topPanel,

				// Inform Myra that external text input is available
				// So it stops translating Keys to chars
				HasExternalTextInput = true
			};

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
		}

		private void PaintHandler(Point size, SKCanvas canvas)
		{
			canvas.Clear(SKColors.CornflowerBlue);

			using (var paint = new SKPaint
			{
				Color = SKColors.Red,
				IsAntialias = true,
				Style = SKPaintStyle.Fill
			})
			{
				canvas.DrawCircle(size.X / 2f, size.Y / 2f, 100, paint);
			}

			using (var font = new SKFont { Size = 64.0f })
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;
				paint.Color = new SKColor(0x42, 0x81, 0xA4);
				paint.IsStroke = false;

				canvas.DrawText("Skia", size.X / 2f, 64.0f, SKTextAlign.Left, font, paint);
			}

			canvas.Flush();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);
			_desktop.Render();
		}
	}
}