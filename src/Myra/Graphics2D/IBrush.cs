#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D
{
	public interface IBrush
	{
		void Draw(SpriteBatch batch, Rectangle dest, Color color);
	}
}
