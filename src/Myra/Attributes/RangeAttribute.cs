using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Specifies the valid range for a numeric property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RangeAttribute : Attribute
	{
		/// <summary>
		/// Gets the minimum allowed value, or null if no minimum.
		/// </summary>
		public float? Minimum { get; }

		/// <summary>
		/// Gets the maximum allowed value, or null if no maximum.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeAttribute"/> class with a minimum value.
		/// </summary>
		/// <param name="min">The minimum allowed value.</param>
		public RangeAttribute(float min): this(min, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeAttribute"/> class with minimum and maximum values.
		/// </summary>
		/// <param name="min">The minimum allowed value.</param>
		/// <param name="max">The maximum allowed value.</param>
		public RangeAttribute(float min, float max) : this((float?)min, (float?)max)
		{
		}
	}
}
