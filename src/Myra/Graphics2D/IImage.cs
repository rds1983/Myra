#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D
{
	public interface IImage: IBrush
	{
		Point Size { get; }
	}
}
