using FontStashSharp.Interfaces;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Platform
{
	public interface IMyraRenderer: IFontStashRenderer
	{
		Rectangle Scissor { get; set; }

		void Begin();

		void End();

		void Draw(object texture, Rectangle dest, Rectangle src, Color color);
	}
}
