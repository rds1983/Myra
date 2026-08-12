using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Specifies an alternative XML name for a class or property during serialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class XmlNameAttribute : Attribute
	{
		/// <summary>
		/// Gets the XML name to use for the decorated member.
		/// </summary>
		public string XmlName { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlNameAttribute"/> class.
		/// </summary>
		/// <param name="xmlName">The XML name to use. Cannot be null or empty.</param>
		public XmlNameAttribute(string xmlName)
		{
			if (string.IsNullOrEmpty(xmlName))
			{
				throw new ArgumentException(nameof(xmlName));
			}

			XmlName = xmlName;
		}
	}
}
