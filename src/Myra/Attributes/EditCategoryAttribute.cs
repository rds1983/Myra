using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class EditCategoryAttribute: Attribute
	{
		private readonly string _name;

		public string Name
		{
			get { return _name; }
		}

		public EditCategoryAttribute(string name)
		{
			_name = name;
		}
	}
}
