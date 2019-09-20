using System.ComponentModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator : IMenuItem
	{
		[DefaultValue(null)]
		public string Id { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Widget Widget { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar { get { return null; } }
	}
}
