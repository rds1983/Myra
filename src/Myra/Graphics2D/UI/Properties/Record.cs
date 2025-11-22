using System;

namespace Myra.Graphics2D.UI.Properties
{
	public abstract class Record
	{
		public bool HasSetter { get; set; }

		public abstract string Name { get; }
		public abstract Type Type { get; }
		public string Category { get; set; }

		public abstract object GetValue(object obj);
		public abstract void SetValue(object obj, object value);

		public abstract T FindAttribute<T>() where T : Attribute;
	}
}
