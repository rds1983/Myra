using System;
using System.Collections.Generic;

namespace Myra.Platform
{
	public struct TouchCollection
	{
		public static readonly TouchCollection Empty = new TouchCollection();

		private List<TouchLocation> _touches;

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

		public bool IsConnected { get; set; }
		
		public int Count { get => Touches.Count; }

		public TouchLocation this[int index] { get => Touches[index]; }
	}
}
