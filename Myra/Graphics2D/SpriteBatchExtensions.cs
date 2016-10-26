using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		private static Texture2D _white;
		private static TextureRegion _whiteRegion;

		private static RasterizerState _uiRasterizerState = new RasterizerState
		{
			ScissorTestEnable = true
		};

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.Game.GraphicsDevice, 1, 1);
					_white.SetData(new[] { Color.White });

					_white.Disposing += (sender, args) =>
					{
						_white = null;
					};
				}

				return _white;
			}
		}

		public static TextureRegion WhiteRegion
		{
			get { return _whiteRegion ?? (_whiteRegion = new TextureRegion(White, new Rectangle(0, 0, 1, 1))); }
		}

		public static void DrawRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, 1, rect.Height));
			FillSolidRect(batch, color, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height));
		}

		public static void FillSolidRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			batch.Draw(White, rect, color);
		}

		internal static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, _uiRasterizerState);
		}

		internal static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}
	}
}
