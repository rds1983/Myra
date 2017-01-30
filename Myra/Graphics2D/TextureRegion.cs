using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public class TextureRegion
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

		public void Draw(SpriteBatch batch, Rectangle dest, Color? color = null)
		{
			batch.Draw(Texture,
				dest,
				Bounds,
				color ?? Color.White);
		}
	}
}