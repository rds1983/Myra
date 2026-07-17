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
		private TextBox _textBox;

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

			var canvas = new SKCanvasWidget
			{
				PaintHandler = PaintHandler
			};

			_textBox = new TextBox
			{
				Text = "Hello, Myra!",
				Width = 200,
				Height = 30
			};

			var topPanel = new HorizontalSplitPane();

			topPanel.Widgets.Add(canvas);
			topPanel.Widgets.Add(_textBox);

			topPanel.SetSplitterPosition(0, 0.75f);

			_desktop = new Desktop
			{
				Root = topPanel
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

				canvas.DrawText(_textBox.Text, size.X / 2f, 64.0f, SKTextAlign.Left, font, paint);
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