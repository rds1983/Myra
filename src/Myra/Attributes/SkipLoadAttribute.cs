using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a property to be skipped during deserialization (loading) operations only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SkipLoadAttribute: Attribute
	{
	}
}
