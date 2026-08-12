using System.Numerics;

namespace Myra.Platform
{
	/// <summary>
	/// Represents a single touch point on a touch-enabled input device.
	/// </summary>
	public struct TouchLocation
	{
		/// <summary>
		/// Gets or sets the position of the touch point.
		/// </summary>
		public Vector2 Position { get; set; }
	}
}
