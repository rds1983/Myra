using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RangeAttribute : Attribute
	{
		public float? Minimum { get; }
		public float? Maximum { get; }

		private RangeAttribute(float? min, float? max)
		{
			if (min != null && max != null && min > max)
			{
				throw new ArgumentException("min > max");
			}

			Minimum = min;
			Maximum = max;
		}

		public RangeAttribute(float min): this(min, null)
		{
		}

		public RangeAttribute(float min, float max) : this((float?)min, (float?)max)
		{
		}
	}
}
