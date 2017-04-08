using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Assets;
using Myra.Graphics2D.Tiled;
using Myra.Graphics2D.UI;

namespace Myra.Samples
{
	public class TiledSample : Game
	{
		private readonly GraphicsDeviceManager graphics;
		private readonly AssetManager _assetManager = new AssetManager(new FileAssetResolver("Assets"));
		private TmxMap _tiledMap;
		private SpriteBatch _batch;
		private TextBlock _drawCallsText;
		private TextBlock _fpsText;
		private Desktop _host;
		private readonly FramesPerSecondCounter _counter = new FramesPerSecondCounter();

		public TiledSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_host = new Desktop();

			var grid = new Grid
			{
				RowSpacing = 8,
				ColumnSpacing = 8
			};

			grid.RowsProportions.Add(new Grid.Proportion());
			grid.RowsProportions.Add(new Grid.Proportion());
			grid.RowsProportions.Add(new Grid.Proportion());

			// Combo
			var combo = new ComboBox();
			combo.Items.Add(new ListItem("desert.tmx"));
			combo.Items.Add(new ListItem("orthogonal-outside.tmx"));
			combo.Items.Add(new ListItem("perspective_walls.tmx"));
			combo.Items.Add(new ListItem("sewers.tmx"));

			combo.SelectedIndexChanged += (s, a) =>
			{
				_tiledMap = _assetManager.Load<TmxMap>("Tiled/" + combo.SelectedItem.Text);
			};

			combo.SelectedIndex = 3;

			grid.Widgets.Add(combo);

			_drawCallsText = new TextBlock
			{
				GridPositionY = 1,
				TextColor = Color.White
			};
			grid.Widgets.Add(_drawCallsText);

			_fpsText = new TextBlock
			{
				GridPositionY = 2,
				TextColor = Color.White
			};
			grid.Widgets.Add(_fpsText);

			_host.Widgets.Add(grid);

			_batch = new SpriteBatch(GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_counter.Update(gameTime);
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
			_tiledMap.Render(_batch);
			_batch.End();

			_counter.Draw(gameTime);

			_drawCallsText.Text = string.Format("Sprites rendered: {0}", GraphicsDevice.Metrics.SpriteCount);
			_fpsText.Text = string.Format("FPS: {0}", _counter.FramesPerSecond);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
