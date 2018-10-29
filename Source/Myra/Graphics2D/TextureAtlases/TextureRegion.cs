using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.TextureAtlases
{
	public class TextureRegion: IRenderable
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
				return Bounds.Size;
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
			batch.Draw(Texture,
				dest,
				Bounds,
				color);
		}
	}
}