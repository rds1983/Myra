#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.Text
{
	public class GlyphInfo
	{
		public int Index;
		public char Character;
		public Rectangle Bounds;
		public TextLine TextLine;
	}
}
