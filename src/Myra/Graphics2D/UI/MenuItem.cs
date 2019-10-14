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
	public class MenuItem : IMenuItem
	{
		private string _shortcutText;
		private Color? _shortcutColor;
		private bool _toggleable;
		private IRenderable _image;
		private char? _underscoreChar;
		private string _id, _text, _displayText;
		private Color? _color;

		internal readonly Image ImageWidget = new Image
		{
			VerticalAlignment = VerticalAlignment.Center
		};

		internal readonly Label Label = new Label(null)
		{
			VerticalAlignment = VerticalAlignment.Center
		};

		internal readonly Label Shortcut = new Label(null)
		{
			VerticalAlignment = VerticalAlignment.Center
		};


		internal readonly Menu SubMenu = new VerticalMenu();

		public string Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				FireChanged();
			}
		}

		[DefaultValue(null)]
		public string Text
		{
			get { return _text; }
			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;

				string text = _text;
				_underscoreChar = null;
				if (value != null)
				{
					var underscoreIndex = value.IndexOf('&');
					if (underscoreIndex >= 0 && underscoreIndex + 1 < value.Length)
					{
						_underscoreChar = char.ToLower(value[underscoreIndex + 1]);
						text = value.Substring(0, underscoreIndex) + value.Substring(underscoreIndex + 1);
					}
				}

				_displayText = text;
			}
		}

		internal string DisplayText
		{
			get
			{
				return _displayText;
			}
		}

		[DefaultValue(null)]
		public Color? Color
		{
			get
			{
				return _color;
			}

			set
			{
				if (value == _color)
				{
					return;
				}

				_color = value;
				FireChanged();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable Image
		{
			get
			{
				return _image;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_image = value;
				FireChanged();
			}
		}

		[DefaultValue(null)]
		public string ShortcutText
		{
			get
			{
				return _shortcutText;
			}

			set
			{
				if (value == _shortcutText)
				{
					return;
				}

				_shortcutText = value;
				FireChanged();
			}
		}

		[DefaultValue(null)]
		public Color? ShortcutColor
		{
			get
			{
				return _shortcutColor;
			}

			set
			{
				if (value == _shortcutColor)
				{
					return;
				}

				_shortcutColor = value;
				FireChanged();
			}
		}

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
			get { return SubMenu.Items; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool Enabled
		{
			get { return ImageWidget.Enabled; }

			set
			{
				ImageWidget.Enabled = Label.Enabled = Shortcut.Enabled = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar
		{
			get
			{
				return _underscoreChar;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool CanOpen
		{
			get
			{
				return Items.Count > 0;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int Index { get; set; }

		public event EventHandler Selected;
		public event EventHandler Changed;

		public MenuItem(string id, string text, Color? color, object tag)
		{
			Id = id;
			Text = text;
			Color = color;
			Tag = tag;
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

			foreach (var item in SubMenu.Items)
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

		protected void FireChanged()
		{
			var ev = Changed;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}