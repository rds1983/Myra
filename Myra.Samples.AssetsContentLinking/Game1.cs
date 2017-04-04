using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;
using Myra.Graphics2D.Text;

namespace Myra.Samples.AssetsContentLinking
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private readonly GraphicsDeviceManager graphics;
		private SpriteBatch _batch;
		private readonly AssetManager _assetManager = new AssetManager(new FileSystemAssetResolver("Assets"));
		private BitmapFont _font;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			Content.RootDirectory = "Content";
			_assetManager.SetAssetLoader(new ContentLoader<Texture2D>(Content));

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

			_font.Draw(_batch, "Hello, World!", Point.Zero, Color.LightGoldenrodYellow);

			_batch.End();
		}
	}
}
