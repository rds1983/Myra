using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Pixmaps
{
	public class PixmapColor : PixmapT<Color>
	{
		public override SurfaceFormat Format
		{
			get { return SurfaceFormat.Color; }
		}

		public PixmapColor(Point size) : base(size)
		{
		}

		public override Color GetColor(Point p)
		{
			return Get(p);
		}

		public override void SetColor(Point p, Color value)
		{
			Set(p, value);
		}
	}
}