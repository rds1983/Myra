using Microsoft.Xna.Framework;
using Myra.Edit;
using Newtonsoft.Json;

namespace Myra.Graphics3D.Environment
{
	public class DirectionalLight : BaseLight
	{
		private Vector3 _direction;
		private Vector3 _normalizedDirection;

		public Vector3 Direction
		{
			get { return _direction; }
			set
			{
				_direction = value;
				UpdateNormalizedDirection();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Vector3 NormalizedDirection
		{
			get { return _normalizedDirection; }
		}

		private void UpdateNormalizedDirection()
		{
			var length = _direction.Length();
			if (length < 0.00001f)
			{
				_normalizedDirection = Vector3.Zero;
			}
			else
			{
				_normalizedDirection = _direction;
				_normalizedDirection.Normalize();
			}
		}
	}
}