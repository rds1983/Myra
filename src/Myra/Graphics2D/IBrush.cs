#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
#endif

namespace Myra.Graphics2D
{
	public interface IBrush
	{
		void Draw(SpriteBatch batch, Rectangle dest, Color color);
	}
}
