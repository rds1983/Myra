using FontStashSharp.Interfaces;
using System;
using System.Drawing;
using System.Numerics;

namespace Myra.Platform
{
	internal class FontStashRenderer: IFontStashRenderer2
	{
		private readonly IMyraRenderer _myraRenderer;

		public ITexture2DManager TextureManager => MyraEnvironment.Platform;

		public FontStashRenderer(IMyraRenderer myraRenderer)
		{
			if (myraRenderer == null)
			{
				throw new ArgumentNullException(nameof(myraRenderer));
			}

			_myraRenderer = myraRenderer;
		}

		public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			_myraRenderer.DrawQuad(texture, ref topLeft, ref topRight, ref bottomLeft, ref bottomRight);
		}
	}
}
