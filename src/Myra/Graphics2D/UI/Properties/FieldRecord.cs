using Myra.Utility;
using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	internal class FieldRecord : Record
	{
		private readonly FieldInfo _fieldInfo;

		public override string Name
		{
			get { return _fieldInfo.Name; }
		}

		public override Type Type
		{
			get { return _fieldInfo.FieldType; }
		}

		public FieldRecord(FieldInfo fieldInfo)
		{
			_fieldInfo = fieldInfo;
		}

		public override object GetValue(object obj)
		{
			return _fieldInfo.GetValue(obj);
		}

		public override void SetValue(object obj, object value)
		{
			_fieldInfo.SetValue(obj, value);
		}

		public override T FindAttribute<T>()
		{
			return _fieldInfo.FindAttribute<T>();
		}
	}
}
