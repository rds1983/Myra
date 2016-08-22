using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;

namespace Myra.Graphics2D.Pixmaps
{
	public class PixmapAlpha8 : PixmapT<byte>
	{
		public override SurfaceFormat Format
		{
			get { return SurfaceFormat.Alpha8; }
		}

		public PixmapAlpha8(Point size) : base(size)
		{
		}

		public override Color GetColor(Point p)
		{
			var c = Get(p);

			return new Color(c, c, c, c);
		}

		public override void SetColor(Point p, Color value)
		{
			Set(p, value.A);
		}
	}
}