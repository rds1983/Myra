using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		internal static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null,
				DefaultAssets.UIRasterizerState);
		}

		internal static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}

		public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle)
		{
			spriteBatch.Draw(textureRegion, destinationRectangle, Color.White);
		}
	}
}
