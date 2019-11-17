#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.Text
{
	internal struct ChunkInfo
	{
		public int X;
		public int Y;
		public int StartIndex;
		public int CharsCount;
		public Color? Color;
		public bool LineEnd;
	}
}