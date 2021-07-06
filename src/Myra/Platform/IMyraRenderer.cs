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
		/// <param name="transformMatrix">A matrix to apply to position, rotation, scale, and depth data passed to Draw.</param>
		/// <param name="textureFiltering"></param>
		void Begin(Matrix3x2? transformMatrix, TextureFiltering textureFiltering);

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


		/// <summary>
		/// Adds a sprite to the batch of sprites to be rendered.
		/// </summary>
		/// <param name="texture">The sprite texture.</param>
		/// <param name="destinationRectangle">A rectangle specifying, in screen coordinates, where the sprite will be drawn.</param>
		/// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
		/// <param name="color">A color mask.</param>
		void Draw(object texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
	}
}
