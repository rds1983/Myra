using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		internal static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, DefaultAssets.UIRasterizerState);
		}

		internal static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}
	}
}
