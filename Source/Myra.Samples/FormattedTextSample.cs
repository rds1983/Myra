using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;

namespace Myra.Samples
{
	public class FormattedTextSample : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private SpriteBatch _batch;
		private FormattedText _formattedText;

		public FormattedTextSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_batch = new SpriteBatch(GraphicsDevice);

			_formattedText = new FormattedText
			{
				Font = DefaultAssets.Font,
				Text =
					"Lorem ipsum [Green]dolor sit amet, [Red]consectetur adipisicing elit, sed do eiusmod [#AAAAAAAA]tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. [white]Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				Width = 500
			};
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
				graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				graphics.ApplyChanges();
			}

			var device = GraphicsDevice;
			device.Clear(Color.Black);

			_batch.Begin();

			_formattedText.Draw(_batch, Point.Zero, Color.LightBlue);

			_batch.End();
		}
	}
}