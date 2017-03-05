using System;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Graphics3D
{
	public class RenderContext
	{
		private Camera _camera;

		public DirectionalLight[] Lights { get; set; }

		public Camera Camera
		{
			get { return _camera; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_camera = value;
			}
		}

		public RenderFlags Flags { get; set; }
	}
}