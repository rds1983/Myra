using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class MenuItem : ListItem, IMenuItemsContainer, IMenuItem
	{
		private readonly ObservableCollection<IMenuItem> _items = new ObservableCollection<IMenuItem>();

		[HiddenInEditor]
		[JsonIgnore]
		public Menu Menu { get; set; }

		[HiddenInEditor]
		public ObservableCollection<IMenuItem> Items
		{
			get { return _items; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public bool Enabled
		{
			get { return Widget.Enabled; }

			set { Widget.Enabled = value; }
		}

		public MenuItem()
		{
		}

		public MenuItem(string text, Color? color, object tag) : base(text, color, tag)
		{
		}

		public MenuItem(string text, Color? color) : this(text, color, null)
		{
		}

		public MenuItem(string text) : this(text, null)
		{
		}
	}
}