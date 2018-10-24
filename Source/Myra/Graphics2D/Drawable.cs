using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D
{
	public class Drawable
	{
		private Color _color = Color.White;

		public TextureRegion TextureRegion { get; set; }

		public Point Size
		{
			get
			{
				return TextureRegion.Bounds.Size;
			}
		}

		public Color Color
		{
			get
			{
				return _color;
			}

			set
			{
				_color = value;
			}
		}

		public Drawable(Texture2D texture)
		{
			TextureRegion = new TextureRegion(texture);
		}

		public Drawable(TextureRegion textureRegion)
		{
			TextureRegion = textureRegion;
		}

		public void Draw(SpriteBatch batch, Vector2 dest, float opacity = 1.0f)
		{
			TextureRegion.Draw(batch, dest, Color * opacity);
		}

		public void Draw(SpriteBatch batch, Rectangle dest, float opacity = 1.0f)
		{
			TextureRegion.Draw(batch, dest, Color * opacity);
		}
	}
}