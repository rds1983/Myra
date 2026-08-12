using System;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Represents a custom name-value pair for use in property editors.
	/// </summary>
	public class CustomValue
	{
		/// <summary>
		/// Gets the name of the custom value.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Gets the value of the custom value.
		/// </summary>
		public object Value { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomValue"/> class.
		/// </summary>
		/// <param name="name">The name of the custom value.</param>
		/// <param name="value">The value associated with the name.</param>
		public CustomValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	/// <summary>
	/// Represents a collection of custom values for use in property editors.
	/// </summary>
	public class CustomValues
	{
		/// <summary>
		/// Gets the array of custom values.
		/// </summary>
		public CustomValue[] Values { get; }
		/// <summary>
		/// Gets or sets the index of the currently selected custom value, or null if none is selected.
		/// </summary>
		public int? SelectedIndex { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomValues"/> class.
		/// </summary>
		/// <param name="values">The enumerable collection of custom values.</param>
		public CustomValues(IEnumerable<CustomValue> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			Values = values.ToArray();
		}
	}
}
