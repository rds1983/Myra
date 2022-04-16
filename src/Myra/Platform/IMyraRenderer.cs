using Myra.Graphics2D;
using System.Drawing;
using System.Numerics;

namespace Myra.Platform
{
	public interface IMyraRenderer
	{
		/// <summary>
		/// A scissor rectangle.
		/// </summary>
		Rectangle Scissor { get; set; }

		/// <summary>
		/// Prepares the graphics device for drawing sprites with specified render state options.
		/// </summary>
		/// <param name="textureFiltering"></param>
		void Begin(TextureFiltering textureFiltering);

		/// <summary>
		/// Flushes the sprite batch.
		/// </summary>
		void End();

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="pos"></param>
		/// <param name="src"></param>
		/// <param name="color"></param>
		/// <param name="rotation"></param>
		/// <param name="origin"></param>
		/// <param name="scale"></param>
		/// <param name="depth"></param>
		void Draw(object texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 origin, Vector2 scale, float depth);
	}
}
