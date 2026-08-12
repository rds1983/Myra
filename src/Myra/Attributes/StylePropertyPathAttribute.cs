using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Specifies the path to a style property that this property is bound to.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StylePropertyPathAttribute : Attribute
	{
		/// <summary>
		/// Gets the path to the style property.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StylePropertyPathAttribute"/> class.
		/// </summary>
		/// <param name="name">The path to the style property.</param>
		public StylePropertyPathAttribute(string name)
		{
			Name = name;
		}
	}
}