using FontStashSharp.Interfaces;
using System.Drawing;
using System.Numerics;

namespace Myra.Platform
{
	public interface IMyraRenderer: IFontStashRenderer
	{
		Rectangle Scissor { get; set; }

		void Begin(Matrix3x2? transform);

		void End();

		void Draw(object texture, Rectangle dest, Rectangle? src, Color color);
	}
}
