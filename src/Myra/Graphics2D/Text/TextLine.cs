using Myra.Utility;
using System;
using System.Collections.Generic;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
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

		public int? GetGlyphIndexByX(int x)
		{
			if (Chunks.Count == 0)
			{
				return null;
			}

			if (Chunks.Count == 1)
			{
				return Chunks[0].GetGlyphIndexByX(x);
			}

			for(var i = 0; i < Chunks.Count; ++i)
			{
				var chunk = Chunks[i];
				if (x >= chunk.Size.X)
				{
					x -= chunk.Size.X;
				}
				else
				{
					return chunk.GetGlyphIndexByX(x);
				}
			}

			// Use last chunk
			return Chunks[Chunks.Count - 1].GetGlyphIndexByX(x);
		}

		public virtual Color Draw(SpriteBatch batch, Point pos, Color color, bool useChunkColor, float opacity = 1.0f)
		{
			foreach (var si in Chunks)
			{
				if (useChunkColor && si.Color != null)
				{
					color = si.Color.Value;
				}

				si.Draw(batch, pos, color, opacity);
				pos.X += si.Size.X;
			}

			return color;
		}
	}
}
