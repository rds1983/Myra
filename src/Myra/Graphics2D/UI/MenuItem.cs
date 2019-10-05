using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System;
using Myra.Attributes;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class MenuItem : SelectableItem, IMenuItemsContainer, IMenuItem
	{
		private bool _toggleable;
		private readonly ObservableCollection<IMenuItem> _items = new ObservableCollection<IMenuItem>();

		[DefaultValue(false)]
		public bool Toggleable
		{
			get
			{
				return _toggleable;
			}

			set
			{
				if (value == _toggleable)
				{
					return;
				}

				_toggleable = value;
				FireChanged();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		[Browsable(false)]
		[Content]
		public ObservableCollection<IMenuItem> Items
		{
			get { return _items; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool Enabled
		{
			get { return Widget != null && Widget.Enabled; }

			set { Widget.Enabled = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Control Widget
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar
		{
			get
			{
				var button = (MenuItemButton)Widget;
				return button.UnderscoreChar;
			}
		}

		public event EventHandler Selected;

		public MenuItem(string id, string text, Color? color, object tag) : base(text, color, tag)
		{
			Id = id;
			Text = text;

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

		public void FireSelected()
		{
			var ev = Selected;

			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}