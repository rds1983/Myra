#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.Text
{
	public class GlyphInfo
	{
		public int Index;
		public char Character;
		public Rectangle Bounds;
		public TextChunk TextChunk;
	}
}
