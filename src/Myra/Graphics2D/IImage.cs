#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
#endif

namespace Myra.Graphics2D
{
	public interface IImage: IBrush
	{
		Point Size { get; }
	}
}
