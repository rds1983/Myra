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
		public Rectangle Bounds { get; private set; }
		public Rectangle? RenderedBounds { get; private set; }

		public GlyphRender(CharInfo charInfo, GlyphRun run, Glyph glyph, Color? color, Rectangle bounds)
		{
			if (run == null)
			{
				throw new ArgumentNullException("run");
			}

			CharInfo = charInfo;
			Run = run;
			Glyph = glyph;
			Color = color;
			Bounds = bounds;
		}

		public void ResetDraw()
		{
			RenderedBounds = null;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color)
		{
			var bounds = Bounds;
			bounds.Offset(pos);

			RenderedBounds = bounds;

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
