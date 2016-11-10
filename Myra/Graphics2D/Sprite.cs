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

		public static Sprite CreateSolidColorRect(Color color)
		{
			return new Sprite(DefaultAssets.WhiteRegion)
			{
				Color = color
			};
		}
	}
}