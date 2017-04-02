using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		public static void DrawRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, 1, rect.Height));
			FillSolidRect(batch, color, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height));
		}

		public static void FillSolidRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			batch.Draw(DefaultAssets.White, rect, color);
		}

		internal static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, DefaultAssets.UIRasterizerState);
		}

		internal static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}
	}
}
