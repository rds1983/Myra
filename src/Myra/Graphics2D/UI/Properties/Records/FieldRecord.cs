using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	internal class FieldRecord : ReflectionRecord
	{
		private readonly FieldInfo _fieldInfo;

		public override string Name => _fieldInfo.Name;
		public override Type Type => _fieldInfo.FieldType;
		public override MemberInfo MemberInfo => _fieldInfo;

		public FieldRecord(FieldInfo fieldInfo)
		{
			_fieldInfo = fieldInfo;
		}

		public override object GetValue(object field)
		{
			return _fieldInfo.GetValue(field);
		}

		public override void SetValue(object field, object value)
		{
			_fieldInfo.SetValue(field, value);
		}
	}
}
