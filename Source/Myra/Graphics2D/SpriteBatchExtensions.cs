using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		internal static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
				DefaultAssets.UIRasterizerState);
		}

		internal static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}

		public static void Draw(this SpriteBatch spriteBatch, TextureRegion textureRegion, Rectangle destinationRectangle, Color? color = null)
		{
			textureRegion.Draw(spriteBatch, destinationRectangle, color);
		}
	}
}
