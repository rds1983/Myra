using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StylePropertyPathAttribute : Attribute
	{
		private readonly string _name;

		public string Name
		{
			get { return _name; }
		}

		public StylePropertyPathAttribute(string name)
		{
			_name = name;
		}
	}
}