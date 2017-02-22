using Microsoft.Xna.Framework;
using Myra.Utility;

namespace Myra.Graphics3D
{
	public sealed class OrthographicCamera: Camera
	{
		private bool _dirty = true;

		private float _near = 1.0f;
		private float _far = 1000.0f;
		private Matrix _projection;

		public float Near
		{
			get { return _near; }
			set
			{
				if (!value.EpsilonEquals(_near))
				{
					_near = value;
					Invalidate();
				}
			}
		}

		public float Far
		{
			get { return _far; }
			set
			{
				if (!value.EpsilonEquals(_far))
				{
					_far = value;
					Invalidate();
				}
			}
		}

		public override Vector2 Viewport
		{
			get
			{
				return base.Viewport;
			}
			set
			{
				if (value != base.Viewport)
				{
					base.Viewport = value;
					Invalidate();
				}
			}
		}

		public override Matrix Projection
		{
			get
			{
				Update();
				return _projection;
			}
		}

		private void Invalidate()
		{
			_dirty = true;
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			_projection = Matrix.CreateOrthographic(Viewport.X, Viewport.Y, _near, _far);

			_dirty = false;
		}
	}
}
