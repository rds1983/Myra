using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI;

namespace Myra.Samples
{
	public class FormattedTextSample : SampleGame
	{
		private SpriteBatch _batch;
		private BitmapFont _font;
		private FormattedText _formattedText;
		private int _y;

		protected override void LoadContent()
		{
			base.LoadContent();

			_batch = new SpriteBatch(GraphicsDevice);
			_font = DefaultAssets.Font;

			_formattedText = new FormattedText
			{
				Font = _font,
				Text =
					"Lorem ipsum [Green]dolor sit amet, [Red]consectetur adipisicing elit, sed do eiusmod [#AAAAAAAA]tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. [white]Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				Size = new Point(500, 500),
				HorizontalAlignment = HorizontalAlignment.Right,
				Wrap = true
			};

			GraphicsDevice.BlendState = BlendState.Additive;
		}

		protected override void Draw(GameTime gameTime)
		{
			var device = GraphicsDevice;
			device.Clear(Color.Black);

			_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			_formattedText.Draw(_batch, new Point(0, ++_y), Color.LightBlue);

			if (_y >= 300)
			{
				_y = 0;
			}

			_batch.End();
		}
	}
}