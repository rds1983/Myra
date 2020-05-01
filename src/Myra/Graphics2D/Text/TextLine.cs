using Myra.Utility;
using System;
using System.Collections.Generic;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class TextLine
	{
		public int Count { get; internal set; }

		public Point Size;

		public int LineIndex
		{
			get; internal set;
		}

		public int Top
		{
			get; internal set;
		}

		public int TextStartIndex
		{
			get; internal set;
		}

		public List<TextChunk> Chunks { get; } = new List<TextChunk>();

		public GlyphInfo GetGlyphInfoByIndex(int index)
		{
			foreach (var si in Chunks)
			{
				if (index >= si.Count)
				{
					index -= si.Count;
				}
				else
				{
					return si.GetGlyphInfoByIndex(index);
				}
			}

			return null;
		}

		public int? GetGlyphIndexByX(int startX)
		{
			if (Chunks.Count == 0)
			{
				return null;
			}

			var x = startX;
			for(var i = 0; i < Chunks.Count; ++i)
			{
				var chunk = Chunks[i];

				if (x >= chunk.Size.X)
				{
					x -= chunk.Size.X;
				}
				else
				{
					if (chunk.Glyphs.Count > 0 && x < chunk.Glyphs[0].Bounds.X)
					{
						// Before first glyph
						return 0;
					}

					return chunk.GetGlyphIndexByX(x);
				}
			}

			// Use last chunk
			x = startX;
			return Chunks[Chunks.Count - 1].GetGlyphIndexByX(startX);
		}

		public Color Draw(SpriteBatch batch, Point pos, Color color, bool useChunkColor, float opacity = 1.0f)
		{
			foreach (var chunk in Chunks)
			{
				if (useChunkColor && chunk.Color != null)
				{
					color = chunk.Color.Value;
				}

				chunk.Draw(batch, pos, color, opacity);
				pos.X += chunk.Size.X;
			}

			return color;
		}
	}
}
