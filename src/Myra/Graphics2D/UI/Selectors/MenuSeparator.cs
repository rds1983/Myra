using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator : IMenuItem
	{
		internal SeparatorWidget Separator;

		[DefaultValue(null)]
		[Browsable(false)]
		[XmlIgnore]
		public string Id { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar { get { return null; } }

		[Browsable(false)]
		[XmlIgnore]
		public int Index { get; set; }
	}
}
