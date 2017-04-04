using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SelectionAttribute: Attribute
	{
		private readonly Type _itemsProviderType;

		public Type ItemsProviderType
		{
			get { return _itemsProviderType; }
		}

		public SelectionAttribute(Type itemsProviderType)
		{
			_itemsProviderType = itemsProviderType;
		}
	}
}
