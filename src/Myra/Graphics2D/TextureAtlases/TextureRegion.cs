using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Texture2D = System.Object;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	public class TextureRegion: IImage
	{
		private readonly Rectangle _bounds;

#if MONOGAME || FNA || STRIDE
		private readonly Texture2D _texture;
		public Texture2D Texture
		{
			get { return _texture; }
		}
#else
		private readonly object _texture;
		public object Texture
		{
			get { return _texture; }
		}
#endif

		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		public Point Size
		{
			get
			{
				return new Point(Bounds.Width, Bounds.Height);
			}
		}

#if MONOGAME || FNA || STRIDE
		/// <summary>
		/// Covers the whole texture
		/// </summary>
		/// <param name="texture"></param>
		public TextureRegion(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height))
		{
		}

#endif

		public TextureRegion(Texture2D texture, Rectangle bounds)
		{
			if (texture == null)
			{
				throw new ArgumentNullException("texture");
			}

			_texture = texture;
			_bounds = bounds;
		}

		public TextureRegion(TextureRegion region, Rectangle bounds)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}

			_texture = region.Texture;
			bounds.Offset(region.Bounds.Location);
			_bounds = bounds;
		}

		public virtual void Draw(RenderContext context, Rectangle dest, Color color)
		{
			context.Draw(Texture, dest, Bounds, color);
		}
	}
}