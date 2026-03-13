using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	internal class PropertyRecord : ReflectionRecord
	{
		private readonly PropertyInfo _propertyInfo;

		public override string Name => _propertyInfo.Name;
		public override Type Type => _propertyInfo.PropertyType;
		public override MemberInfo MemberInfo => _propertyInfo;

		public PropertyRecord(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public override object GetValue(object field)
		{
			return _propertyInfo.GetValue(field, Array.Empty<object>());
		}

		public override void SetValue(object field, object value)
		{
			_propertyInfo.SetValue(field, value);
		}
	}
}
