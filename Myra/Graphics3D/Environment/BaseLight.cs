using Microsoft.Xna.Framework;

namespace Myra.Graphics3D.Environment
{
	public abstract class BaseLight
	{
		public Color Color { get; set; }

		protected BaseLight()
		{
			Color = Color.White;
		}
	}
}
