#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Represents an image that can be drawn and has a defined size.
	/// </summary>
	public interface IImage : IBrush
	{
		/// <summary>
		/// Gets the size of the image in pixels.
		/// </summary>
		Point Size { get; }
	}
}
