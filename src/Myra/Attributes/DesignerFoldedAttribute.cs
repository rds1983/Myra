using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a property as initially folded (collapsed) when displayed in a designer or property grid.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DesignerFoldedAttribute: Attribute
	{
	}
}
