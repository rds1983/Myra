using System;

namespace Myra.Attributes
{
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
