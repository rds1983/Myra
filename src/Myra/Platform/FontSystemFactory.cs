using FontStashSharp;

namespace Myra.Platform
{
	public static class FontSystemFactory
	{
		private static FontSystem InternalCreate(int textureWidth, int textureHeight, int blurAmount, int strokeAmount, bool premultiplyAlpha)
		{
			var result = new FontSystem(StbTrueTypeSharpFontLoader.Instance, MyraEnvironment.Platform, textureWidth, textureHeight, blurAmount, strokeAmount, premultiplyAlpha);

			return result;
		}

		public static FontSystem Create(int textureWidth = 1024, int textureHeight = 1024, bool premultiplyAlpha = true)
		{
			return InternalCreate(textureWidth, textureHeight, 0, 0, premultiplyAlpha);
		}

		public static FontSystem CreateBlurry(int blurAmount, int textureWidth = 1024, int textureHeight = 1024, bool premultiplyAlpha = true)
		{
			return InternalCreate(textureWidth, textureHeight, blurAmount, 0, premultiplyAlpha);
		}
		public static FontSystem CreateStroked(int strokeAmount, int textureWidth = 1024, int textureHeight = 1024, bool premultiplyAlpha = true)
		{
			return InternalCreate(textureWidth, textureHeight, 0, strokeAmount, premultiplyAlpha);
		}
	}
}
