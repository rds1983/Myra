using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.UI
{
	public class RenderContext
	{
		public SpriteBatch Batch { get; set; }
		public Rectangle View { get; set; }
		public float Opacity { get; set; }
	}
}