using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
using Myra.Platform;
using Color = FontStashSharp.FSColor;
using Texture2D = System.Object;
using Matrix = System.Numerics.Matrix3x2;
using FontStashSharp.Interfaces;
#endif

namespace Myra.Utility
{
	internal static class GraphicsExtension
	{
		public static Point Size(this Rectangle r)
		{
			return new Point(r.Width, r.Height);
		}

#if PLATFORM_AGNOSTIC
		public static Vector3 TransformToVector3(this Vector2 v, ref Matrix matrix, float z)
		{
			var result = v.Transform(ref matrix);
			return new Vector3(result.X, result.Y, z);
		}

		public static void DrawQuad(this IMyraRenderer renderer,
			Texture2D texture, Color color,
			Vector2 baseOffset, ref Matrix transformation, float layerDepth,
			Vector2 size, Rectangle textureRectangle,
			ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight,
			ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			var textureSize = renderer.TextureManager.GetTextureSize(texture);

			topLeft.Position = baseOffset.TransformToVector3(ref transformation, layerDepth);
			topLeft.TextureCoordinate = new Vector2((float)textureRectangle.X / textureSize.X,
													(float)textureRectangle.Y / textureSize.Y);
			topLeft.Color = color;

			topRight.Position = (baseOffset + new Vector2(size.X, 0)).TransformToVector3(ref transformation, layerDepth);
			topRight.TextureCoordinate = new Vector2((float)textureRectangle.Right / textureSize.X,
												 (float)textureRectangle.Y / textureSize.Y);
			topRight.Color = color;

			bottomLeft.Position = (baseOffset + new Vector2(0, size.Y)).TransformToVector3(ref transformation, layerDepth);
			bottomLeft.TextureCoordinate = new Vector2((float)textureRectangle.Left / textureSize.X,
														 (float)textureRectangle.Bottom / textureSize.Y);
			bottomLeft.Color = color;

			bottomRight.Position = (baseOffset + new Vector2(size.X, size.Y)).TransformToVector3(ref transformation, layerDepth);
			bottomRight.TextureCoordinate = new Vector2((float)textureRectangle.Right / textureSize.X,
														(float)textureRectangle.Bottom / textureSize.Y);
			bottomRight.Color = color;

			renderer.DrawQuad(texture, ref topLeft, ref topRight, ref bottomLeft, ref bottomRight);
		}

#endif
	}
}