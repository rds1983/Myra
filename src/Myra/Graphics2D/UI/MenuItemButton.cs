using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using System;
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	internal class MenuItemButton : ButtonBase<HorizontalStackPanel>
	{
		private readonly Proportion _imageProportion = new Proportion(ProportionType.Pixels, 0);
		private int _imageWidth = 0;
		private readonly Image _image;
		private readonly Label _label;
		private readonly Label _shortcut;
		private readonly Menu _subMenu = new VerticalMenu();
		private char? _underscoreChar;
		private string _text;

		internal int ImageWidth
		{
			get
			{
				return _imageWidth;
			}

			set
			{
				if (_imageWidth == value)
				{
					return;
				}

				_imageWidth = value;
				_imageProportion.Value = value;
			}
		}

		internal Image ImageWidget
		{
			get
			{
				return _image;
			}
		}

		public int LabelWidth
		{
			get
			{
				return _label.ActualBounds.Width;
			}
		}

		public int? LabelSetWidth
		{
			set
			{
				_label.Width = value;
			}
		}

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

				_label.Text = text;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public SpriteFont Font
		{
			get { return _label.Font; }
			set { _label.Font = value; }
		}

		public Color TextColor
		{
			get { return _label.TextColor; }
			set { _label.TextColor = value; }
		}

		internal char? UnderscoreChar
		{
			get
			{
				return _underscoreChar;
			}
		}

		public string ShortcutText
		{
			get { return _shortcut.Text; }
			set
			{
				_shortcut.Text = value;
				UpdateShortcut();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public SpriteFont ShortcutFont
		{
			get { return _shortcut.Font; }
			set
			{
				_shortcut.Font = value;
				UpdateShortcut();
			}
		}

		private bool CanHaveShortcut
		{
			get
			{
				return !string.IsNullOrEmpty(ShortcutText) && ShortcutFont != null;
			}
		}

		public Color ShortcutTextColor
		{
			get { return _shortcut.TextColor; }
			set { _shortcut.TextColor = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable Image
		{
			get { return _image.Renderable; }
			set { _image.Renderable = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable OpenBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Menu SubMenu
		{
			get { return _subMenu; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool CanOpen
		{
			get
			{
				return _subMenu != null && SubMenu.Items != null && SubMenu.Items.Count > 0;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get { return base.ContentHorizontalAlignment; }
			set { base.ContentHorizontalAlignment = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get { return base.ContentVerticalAlignment; }
			set { base.ContentVerticalAlignment = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public override bool Toggleable
		{
			get { return base.Toggleable; }

			set
			{
				base.Toggleable = value;
			}
		}

		private Menu Menu
		{
			get; set;
		}

		internal bool HasImage
		{
			get
			{
				return InternalChild.Widgets[0] == _image;
			}

			set
			{
				var hasImage = HasImage;
				if (value == hasImage)
				{
					return;
				}

				if (hasImage && !value)
				{
					InternalChild.Proportions.RemoveAt(0);
					InternalChild.Widgets.RemoveAt(0);
				}
				else if (!hasImage && value)
				{
					InternalChild.Proportions.Insert(0, _imageProportion);
					InternalChild.Widgets.Insert(0, _image);
				}
			}
		}

		protected override bool UseHoverRenderable
		{
			get
			{
				if (Menu != null && Menu.KeyboardHoverButton != null)
				{
					return Menu.KeyboardHoverButton == this;
				}

				return base.UseHoverRenderable;
			}
		}

		internal MenuItemButton(Menu menu, MenuItemStyle style)
		{
			Menu = menu;
			InternalChild = new HorizontalStackPanel();

			InternalChild.Proportions.Add(Proportion.Fill);

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};

			_label = new Label(null);
			InternalChild.Widgets.Add(_label);

			_shortcut = new Label(null);
			InternalChild.Widgets.Add(_shortcut);

			if (style != null)
			{
				ApplyMenuItemStyle(style);
			}
		}

		public void ApplyMenuItemStyle(MenuItemStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				_image.ApplyPressableImageStyle(style.ImageStyle);
			}

			if (style.LabelStyle != null)
			{
				_label.ApplyLabelStyle(style.LabelStyle);
			}

			if (style.ShortcutStyle != null)
			{
				_shortcut.ApplyLabelStyle(style.ShortcutStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();
			_label.IsPressed = IsPressed;
		}

		private void UpdateShortcut()
		{
			if (CanHaveShortcut && !InternalChild.Widgets.Contains(_shortcut))
			{
				InternalChild.Widgets.Add(_shortcut);
			} else if (!CanHaveShortcut && InternalChild.Widgets.Contains(_shortcut))
			{
				InternalChild.Widgets.Remove(_shortcut);
			}
		}
	}
}