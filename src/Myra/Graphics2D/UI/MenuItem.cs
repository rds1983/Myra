using System.Collections.ObjectModel;
using Myra.Attributes;
using Newtonsoft.Json;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

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
			get { return Widget != null && Widget.Enabled; }

			set { Widget.Enabled = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public char? UnderscoreChar
		{
			get
			{
				var button = (MenuItemButton)Widget;
				return button.UnderscoreChar;
			}
		}

		public MenuItem(string id, string text, Color? color, object tag) : base(text, color, tag)
		{
			Id = id;
		}

		public MenuItem(string id, string text, Color? color) : this(id, text, color, null)
		{
		}

		public MenuItem(string id, string text) : this(id, text, null)
		{
		}

		public MenuItem(string id) : this(id, string.Empty)
		{
		}

		public MenuItem() : this(string.Empty)
		{
		}

		internal MenuItem FindMenuItemById(string id)
		{
			if (Id == id)
			{
				return this;
			}

			foreach (var item in _items)
			{
				var asMenuItem = item as MenuItem;
				if (asMenuItem == null)
				{
					continue;
				}

				var result = asMenuItem.FindMenuItemById(id);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}
	}
}