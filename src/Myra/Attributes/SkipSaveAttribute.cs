using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Determines that property shouldn't be saved during MML serialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SkipSaveAttribute: Attribute
	{
	}
}
