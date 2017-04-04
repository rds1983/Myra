using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class HiddenInEditorAttribute: Attribute
	{
	}
}
