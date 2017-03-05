using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Graphics3D
{
	public class Scene
	{
		private readonly List<BaseObject> _items = new List<BaseObject>();
		private readonly RenderContext _context = new RenderContext();

		public List<BaseObject> Items
		{
			get { return _items; }
		}

		public int ModelsRendered { get; private set; }

		public void Render(Camera camera, DirectionalLight[] lights, RenderFlags flags = RenderFlags.None)
		{
			_context.Camera = camera;
			_context.Lights = lights;
			_context.Flags = flags;

			var _device = MyraEnvironment.GraphicsDevice;

			var oldDepthStencilState = _device.DepthStencilState;
			var oldRasterizerState = _device.RasterizerState;

			try
			{
				_device.DepthStencilState = DepthStencilState.Default;
				_device.RasterizerState = RasterizerState.CullCounterClockwise;

				// Set the View matrix which defines the camera and what it's looking at
				camera.Viewport = new Vector2(_device.Viewport.Width, _device.Viewport.Height);

				var modelsRendered = 0;
				var frustrum = new BoundingFrustum(camera.ViewProjection);
				// Apply the effect and render items
				foreach (var item in _items)
				{
					if (!frustrum.Intersects(item.BoundingBox))
					{
						// Item is not visible
						continue;
					}

					item.Render(_context);

					++modelsRendered;
				}

				ModelsRendered = modelsRendered;
			}
			finally
			{
				_device.DepthStencilState = oldDepthStencilState;
				_device.RasterizerState = oldRasterizerState;
			}
		}
	}
}
