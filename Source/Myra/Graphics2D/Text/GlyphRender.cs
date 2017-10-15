using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Text
{
	public class GlyphRender
	{
		public CharInfo CharInfo { get; private set; }
		public GlyphRun Run { get; private set; }
		public BitmapFontGlyph Glyph { get; set; }
		public Color? Color { get; private set; }
		public Rectangle? RenderedBounds { get; private set; }

		public int XAdvance
		{
			get { return Glyph.FontRegion != null ? Glyph.FontRegion.XAdvance : 0; }
		}

		public int Width
		{
			get { return Glyph.FontRegion != null ? Glyph.FontRegion.Width + Glyph.FontRegion.XOffset : 0; }
		}

		public int Height
		{
			get { return Glyph.FontRegion != null ? Glyph.FontRegion.Height + Glyph.FontRegion.YOffset : 0; }
		}

		public GlyphRender(CharInfo charInfo, GlyphRun run, Color? color)
		{
			if (run == null)
			{
				throw new ArgumentNullException("run");
			}

			CharInfo = charInfo;
			Run = run;
			Color = color;
		}

		public void ResetDraw()
		{
			RenderedBounds = null;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color)
		{
			if (Glyph.FontRegion == null)
			{
				return;
			}

			var region = Glyph.FontRegion;
			var v = new Vector2(pos.X + region.XOffset, pos.Y + region.YOffset);

			var glyphColor = Color ?? color;

			batch.Draw(region.TextureRegion, v, glyphColor);

			var bounds = new Rectangle((int)v.X, (int)v.Y, region.Width, region.Height);
			if (bounds.Width == 0 || bounds.Height == 0)
			{
				bounds.X = (int)v.X;
				bounds.Y = (int)v.Y;
				bounds.Width = XAdvance;
				bounds.Height = Run.BitmapFont.LineHeight;
				bounds.Offset(pos);
			}

			RenderedBounds = bounds;
		}
	}
}
