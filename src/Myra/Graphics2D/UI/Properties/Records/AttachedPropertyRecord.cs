using Myra.MML;
using System;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
	internal class AttachedPropertyRecord : Record
	{
		private readonly BaseAttachedPropertyInfo _property;
		
		public AttachedPropertyRecord(BaseAttachedPropertyInfo property)
		{
			_property = property ?? throw new ArgumentNullException(nameof(property));
			HasSetter = true;
		}

		public override string Name => _property.Name;

		public override Type Type => _property.PropertyType;
		public override object GetValue(object field) => _property.GetValueObject((Widget)field);
		public override void SetValue(object field, object value) => _property.SetValueObject((Widget)field, value);

		public override T FindAttribute<T>()
		{
			if (_property.Attributes == null)
			{
				return null;
			}

			return (T)(from a in _property.Attributes where a.GetType() == typeof(T) select a).FirstOrDefault();
		}
	}
}
