using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Myra.Assets;

namespace Myra.Samples
{
	public class CustomFontSample : Game
	{
		private readonly GraphicsDeviceManager graphics;
		private SpriteBatch _batch;
		private readonly AssetManager _assetManager = new AssetManager(new FileAssetResolver("Assets"));
		private BitmapFont _font;

		public CustomFontSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_batch = new SpriteBatch(GraphicsDevice);
			_font = _assetManager.Load<BitmapFont>("mistral.fnt");
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

			_batch.DrawString(_font, "Hello, World!", Vector2.Zero, Color.Green);

			_batch.End();
		}
	}
}