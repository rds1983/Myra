using Microsoft.Xna.Framework;

namespace Myra.Graphics3D
{
	public abstract class BaseObject
	{
		public string Id { get; set; }

		public abstract BoundingBox BoundingBox { get; }

		public abstract void Render(RenderContext context);
	}
}
