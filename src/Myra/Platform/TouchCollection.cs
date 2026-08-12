using System;
using System.Collections.Generic;

namespace Myra.Platform
{
	/// <summary>
	/// Represents a collection of touch locations from a touch-enabled input device.
	/// </summary>
	public struct TouchCollection
	{
		/// <summary>
		/// Gets an empty touch collection.
		/// </summary>
		public static readonly TouchCollection Empty = new TouchCollection();

		private List<TouchLocation> _touches;

		/// <summary>
		/// Gets or sets the list of touch locations in the collection.
		/// </summary>
		public List<TouchLocation> Touches
		{
			get => _touches;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_touches = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a touch input device is connected.
		/// </summary>
		public bool IsConnected { get; set; }

		/// <summary>
		/// Gets the number of touch locations in the collection.
		/// </summary>
		public int Count { get => Touches.Count; }

		/// <summary>
		/// Gets the touch location at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the touch location.</param>
		/// <returns>The touch location at the specified index.</returns>
		public TouchLocation this[int index] { get => Touches[index]; }
	}
}
