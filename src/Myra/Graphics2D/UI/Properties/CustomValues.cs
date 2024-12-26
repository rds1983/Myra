using System;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
	public class CustomValue
	{
		public string Name { get; }
		public object Value { get; }

		public CustomValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	public class CustomValues
	{
		public CustomValue[] Values { get; }
		public int? SelectedIndex { get; set; }

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
