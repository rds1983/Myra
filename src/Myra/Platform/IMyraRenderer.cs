using FontStashSharp.Interfaces;
using Myra.Graphics2D;
using System.Drawing;

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
		/// Draws a texture quad
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="topLeft"></param>
		/// <param name="topRight"></param>
		/// <param name="bottomLeft"></param>
		/// <param name="bottomRight"></param>
		void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight);
	}
}
