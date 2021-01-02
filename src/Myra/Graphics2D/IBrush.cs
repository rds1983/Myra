#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	public interface IBrush
	{
		void Draw(RenderContext context, Rectangle dest, Color color);
	}
}
