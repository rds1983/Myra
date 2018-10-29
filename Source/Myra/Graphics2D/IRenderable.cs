using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public interface IRenderable
	{
		Point Size { get; }

		void Draw(SpriteBatch batch, Rectangle dest, Color color);
	}
}
