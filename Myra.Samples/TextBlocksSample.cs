using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment;

namespace Myra.Samples
{
	public class TextBlocksSample : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Desktop _host;

		public TextBlocksSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_host = new Desktop();

			var textBlock1 = new TextBlock
			{
				Text = "Hello, World!",
				TextColor = Color.Red,
				XHint = 100,
				YHint = 50
			};

			var textBlock2 = new TextBlock
			{
				Text = "Hello, World!",
				TextColor = Color.Green,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Font = DefaultAssets.FontSmall,
				PaddingLeft = 16,
				PaddingRight = 8,
				PaddingTop = 16,
				PaddingBottom = 8
			};

			var textBlock3 = new TextBlock
			{
				Text = "Hello, [Red]World!",
				TextColor = Color.Blue,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Bottom,
				WidthHint = 50
			};

			_host.Widgets.Add(textBlock1);
			_host.Widgets.Add(textBlock2);
			_host.Widgets.Add(textBlock3);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
