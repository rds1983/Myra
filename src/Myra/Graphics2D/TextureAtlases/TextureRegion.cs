using System;
using Myra.Assets;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Texture2D = Xenko.Graphics.Texture;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	[AssetLoader(typeof(TextureRegionLoader))]
	public class TextureRegion: IImage
	{
		private readonly Texture2D _texture;
		private readonly Rectangle _bounds;

		public Texture2D Texture
		{
			get { return _texture; }
		}

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

		/// <summary>
		/// Covers the whole texture
		/// </summary>
		/// <param name="texture"></param>
		public TextureRegion(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height))
		{
		}

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

		public virtual void Draw(SpriteBatch batch, Rectangle dest, Color color)
		{
#if !XENKO
			batch.Draw(Texture,
				dest,
				Bounds,
				color);
#else
			batch.Draw(Texture,
				new RectangleF(dest.X, dest.Y, dest.Width, dest.Height),
				new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height),
				color,
				0.0f,
				Vector2.Zero);
#endif
		}
	}
}