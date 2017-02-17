using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Text
{
	public class GlyphRender
	{
		public BitmapFont Font { get; private set; }
		public CharInfo CharInfo { get; private set; }
		public GlyphRun Run { get; private set; }
		public Glyph Glyph { get; private set; }
		public Color? Color { get; private set; }
		public Point Location { get; private set; }
		public Rectangle? RenderedBounds { get; private set; }

		public int XAdvance
		{
			get { return Glyph != null ? Glyph.XAdvance : 0; }
		}

		public GlyphRender(BitmapFont font, CharInfo charInfo, GlyphRun run, Glyph glyph, Color? color, Point location)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			if (run == null)
			{
				throw new ArgumentNullException("run");
			}

			Font = font;
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
			if (Glyph == null || Glyph.Region == null)
			{
				return;
			}

			var bounds = new Rectangle(Location.X + Glyph.Offset.X, 
				Location.Y + Glyph.Offset.Y,
				Glyph.Region.Bounds.Width,
				Glyph.Region.Bounds.Height);

			bounds.Offset(pos);

			var glyphColor = Color ?? color;
			Glyph.Region.Draw(batch, bounds, glyphColor);

			if (BitmapFont.DrawFames)
			{
				batch.DrawRect(Microsoft.Xna.Framework.Color.YellowGreen, bounds);
			}

			if (bounds.Width == 0 || bounds.Height == 0)
			{
				bounds.X = Location.X;
				bounds.Y = Location.Y;
				bounds.Width = XAdvance;
				bounds.Height = Font.LineHeight;
				bounds.Offset(pos);
			}

			RenderedBounds = bounds;

		}
	}
}
