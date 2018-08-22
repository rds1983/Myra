using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI
{
	public class RenderContext
	{
		public SpriteBatch Batch { get; set; }
		public Rectangle View { get; set; }
		public float Opacity { get; set; }

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="textureRegion"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(TextureRegion textureRegion, Rectangle rectangle, Color color)
		{
			Batch.Draw(textureRegion, rectangle, color * Opacity);
		}

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="textureRegion"></param>
		/// <param name="pos"></param>
		/// <param name="color"></param>
		public void Draw(TextureRegion textureRegion, Vector2 pos, Color color)
		{
			Batch.Draw(textureRegion, pos, color * Opacity);
		}

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="textureRegion"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(TextureRegion textureRegion, Rectangle rectangle)
		{
			Batch.Draw(textureRegion, rectangle, Color.White * Opacity);
		}

		/// <summary>
		/// Draws a hollow rectangle taking into account the context transformations
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void DrawRectangle(Rectangle rectangle, Color color)
		{
			Batch.DrawRectangle(rectangle, color * Opacity);
		}

		/// <summary>
		/// Draws a filled rectangle taking into account the context transformations
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(Rectangle rectangle, Color color)
		{
			Batch.FillRectangle(rectangle, color * Opacity);
		}
	}
}