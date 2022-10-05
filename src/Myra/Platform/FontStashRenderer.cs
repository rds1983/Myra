using FontStashSharp.Interfaces;
using System;
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;

namespace Myra.Platform
{
	internal class FontStashRenderer: IFontStashRenderer
	{
		private readonly IMyraRenderer _myraRenderer;

		public ITexture2DManager TextureManager => MyraEnvironment.Platform.Renderer.TextureManager;

		public FontStashRenderer(IMyraRenderer myraRenderer)
		{
			if (myraRenderer == null)
			{
				throw new ArgumentNullException(nameof(myraRenderer));
			}

			_myraRenderer = myraRenderer;
		}

		public void Draw(object texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 scale, float depth)
		{
			_myraRenderer.DrawSprite(texture, pos, src, color, rotation, scale, depth);
		}
	}
}
