using Myra.MML;
using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	internal class AttachedPropertyRecord : Record
	{
		private readonly BaseAttachedPropertyInfo _property;

		public override MemberInfo MemberInfo => null;

		public AttachedPropertyRecord(BaseAttachedPropertyInfo property)
		{
			_property = property ?? throw new ArgumentNullException(nameof(property));
			HasSetter = true;
		}

		public override string Name => _property.Name;

		public override Type Type => _property.PropertyType;
		public override object GetValue(object obj) => _property.GetValueObject((Widget)obj);

		public override void SetValue(object obj, object value) => _property.SetValueObject((Widget)obj, value);
	}
}
