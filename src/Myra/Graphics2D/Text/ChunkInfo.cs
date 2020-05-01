#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
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