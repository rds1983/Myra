using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;

namespace Myra.Graphics2D
{
	public abstract class ImageDrawable: Drawable
	{
		public Color Color { get; set; }
		public Point Size
		{
			get { return TextureRegion.Bounds.Size; }
		}

		public TextureRegion TextureRegion { get; private set; }

		protected ImageDrawable(TextureRegion region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}

			TextureRegion = region;
			Color = Color.White;
		}

		public abstract void Draw(SpriteBatch batch, Rectangle dest);
	}
}
