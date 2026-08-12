using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A separator menu item that visually divides menu items.
	/// </summary>
	public class MenuSeparator : IMenuItem
	{
		internal SeparatorWidget Separator;

		/// <summary>
		/// Gets or sets the identifier of the menu separator.
		/// </summary>
		[DefaultValue(null)]
		[Browsable(false)]
		[XmlIgnore]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the menu that contains this separator.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		/// <summary>
		/// Gets the underscore character position for the separator (always null).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar { get { return null; } }

		/// <summary>
		/// Gets or sets the index of this separator within its parent menu.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Index { get; set; }
	}
}
