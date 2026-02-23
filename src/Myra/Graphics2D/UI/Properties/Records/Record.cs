using Myra.Utility;
using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Base for encapsulating reflective information using <see cref="MemberInfo"/>.
	/// </summary>
	public abstract class Record
	{
		public bool HasSetter { get; set; }
		public string Category { get; set; }
		public abstract string Name { get; }
		public abstract Type Type { get; }
		public virtual MemberInfo MemberInfo { get; }

		public abstract object GetValue(object field);
		public abstract void SetValue(object field, object value);

		public abstract T FindAttribute<T>() where T : Attribute;
	}
}
