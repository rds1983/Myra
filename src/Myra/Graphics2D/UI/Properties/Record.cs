using Myra.Utility;
using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	public abstract class Record
	{
		public bool HasSetter { get; set; }

		public abstract string Name { get; }
		public abstract Type Type { get; }
		public string Category { get; set; }
		public abstract MemberInfo MemberInfo { get; }

		public abstract object GetValue(object obj);
		public abstract void SetValue(object obj, object value);

		public T FindAttribute<T>() where T : Attribute
		{
			if (MemberInfo == null)
			{
				return null;
			}

			return MemberInfo.FindAttribute<T>();
		}

		public T[] FindAttributes<T>() where T: Attribute
		{
			if (MemberInfo == null)
			{
				return null;
			}

			return MemberInfo.FindAttributes<T>();
		}
	}
}
