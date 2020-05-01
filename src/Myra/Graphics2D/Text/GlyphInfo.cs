#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
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
