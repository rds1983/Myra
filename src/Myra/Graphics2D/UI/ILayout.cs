using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public interface ILayout
	{
		Point Measure(IEnumerable<Widget> widgets, Point availableSize);
		void Arrange(IEnumerable<Widget> widgets, Rectangle bounds);
	}
}
