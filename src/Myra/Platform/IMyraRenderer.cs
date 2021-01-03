using FontStashSharp.Interfaces;
using System.Drawing;

namespace Myra.Platform
{
	public interface IMyraRenderer: IFontStashRenderer
	{
		Rectangle Scissor { get; set; }

		void Begin();

		void End();

		void Draw(object texture, Rectangle dest, Rectangle? src, Color color);
	}
}
