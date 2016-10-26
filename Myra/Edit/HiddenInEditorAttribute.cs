using System;

namespace Myra.Edit
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class HiddenInEditorAttribute: Attribute
	{
	}
}
