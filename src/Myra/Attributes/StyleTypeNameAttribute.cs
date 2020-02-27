using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class StyleTypeNameAttribute: Attribute
	{
		private readonly string _name;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public StyleTypeNameAttribute(string name)
		{
			_name = name;
		}
	}
}
