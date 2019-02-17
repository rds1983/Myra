using Myra.Attributes;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator : IMenuItem
	{
		[DefaultValue(null)]
		public string Id { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Menu Menu { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Widget Widget { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public char? UnderscoreChar { get { return null; } }
	}
}
