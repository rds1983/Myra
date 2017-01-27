using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Text
{
	public class GlyphRender
	{
		public CharInfo CharInfo { get; private set; }
		public GlyphRun Run { get; private set; }
		public Glyph Glyph { get; private set; }
		public Color? Color { get; private set; }
		public Point Location { get; private set; }
		public Rectangle? RenderedBounds { get; private set; }

		public GlyphRender(CharInfo charInfo, GlyphRun run, Glyph glyph, Color? color, Point location)
		{
			if (run == null)
			{
				throw new ArgumentNullException("run");
			}

			CharInfo = charInfo;
			Run = run;
			Glyph = glyph;
			Color = color;
			Location = location;
		}

		public void ResetDraw()
		{
			RenderedBounds = null;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color)
		{
			var bounds = new Rectangle(Location.X, Location.Y,
				Glyph != null ? Glyph.Region.Bounds.Width : 0,
				Glyph != null ? Glyph.Region.Bounds.Height : 0);
			bounds.Offset(pos);

			RenderedBounds = new Rectangle(bounds.X, bounds.Y,
				Glyph != null ? Glyph.XAdvance : 0,
				bounds.Height);

			if (Glyph == null)
			{
				return;
			}

			var glyphColor = Color ?? color;

			Glyph.Region.Draw(batch, bounds, glyphColor);

			if (BitmapFont.DrawFames)
			{
				batch.DrawRect(Microsoft.Xna.Framework.Color.YellowGreen, RenderedBounds.Value);
			}
		}
	}
}
