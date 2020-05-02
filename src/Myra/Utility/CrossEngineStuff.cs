using System.IO;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#endif

namespace Myra.Utility
{
	internal static class CrossEngineStuff
	{
		public static Texture2D CreateTexture2D(int width, int height)
		{
#if !STRIDE
			return new Texture2D(MyraEnvironment.GraphicsDevice, width, height, false, SurfaceFormat.Color);
#else
			return Texture2D.New2D(MyraEnvironment.GraphicsDevice, width, height, false, PixelFormat.R8G8B8A8_UNorm_SRgb, TextureFlags.ShaderResource);
#endif
		}

		public static void SetData<T>(Texture2D texture, T[] data) where T: struct
		{
#if !STRIDE
			texture.SetData(data);
#else
			var commandList = MyraEnvironment.Game.GraphicsContext.CommandList;

			texture.SetData(commandList, data);
#endif
		}

		public static int LineSpacing(SpriteFont font)
		{
#if !STRIDE
			return font.LineSpacing;
#else
			return (int)font.Size;
#endif
		}

		public static Rectangle GetScissor()
		{
#if !STRIDE
			var rect = MyraEnvironment.GraphicsDevice.ScissorRectangle;

			rect.X -= MyraEnvironment.GraphicsDevice.Viewport.X;
			rect.Y -= MyraEnvironment.GraphicsDevice.Viewport.Y;

			return rect;
#else
			return MyraEnvironment.Game.GraphicsContext.CommandList.Scissor;
#endif
		}

		public static void SetScissor(Rectangle rect)
		{
#if !STRIDE
			rect.X += MyraEnvironment.GraphicsDevice.Viewport.X;
			rect.Y += MyraEnvironment.GraphicsDevice.Viewport.Y;
			MyraEnvironment.GraphicsDevice.ScissorRectangle = rect;
#else
			MyraEnvironment.Game.GraphicsContext.CommandList.SetScissorRectangle(rect);
#endif
		}

		public static Point ViewSize(this GraphicsDevice device)
		{
#if !STRIDE
			return new Point(device.Viewport.Width, device.Viewport.Height);
#else
			return new Point(device.Presenter.BackBuffer.Width, device.Presenter.BackBuffer.Height);
#endif
		}
	}
}
