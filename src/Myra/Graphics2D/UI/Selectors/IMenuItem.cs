using Myra.MML;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents a menu item that can be displayed in a menu.
	/// </summary>
	public interface IMenuItem: IItemWithId
	{
		/// <summary>
		/// Gets or sets the menu that contains this menu item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		Menu Menu { get; set; }

		/// <summary>
		/// Gets the underscore character position (mnemonic character) in the menu item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		char? UnderscoreChar { get; }

		/// <summary>
		/// Gets or sets the index of this menu item within its parent menu.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		int Index { get; set; }
	}
}
