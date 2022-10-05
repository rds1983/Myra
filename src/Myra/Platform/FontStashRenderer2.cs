using FontStashSharp.Interfaces;
using System;

namespace Myra.Platform
{
	internal class FontStashRenderer2: IFontStashRenderer2
	{
		private readonly IMyraRenderer _myraRenderer;

		public ITexture2DManager TextureManager => MyraEnvironment.Platform.Renderer.TextureManager;

		public FontStashRenderer2(IMyraRenderer myraRenderer)
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
