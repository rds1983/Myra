using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public class Sprite : ImageDrawable
	{
		public Sprite(TextureRegion region) : base(region)
		{
		}

		public override void Draw(SpriteBatch batch, Rectangle dest)
		{
			if (TextureRegion == null)
			{
				return;
			}

			TextureRegion.Draw(batch, dest, Color);
		}
	}
}