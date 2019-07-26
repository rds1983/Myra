using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	internal class MenuItemButton : ButtonBase<Grid>
	{
		private readonly Grid.Proportion _imageProportion;
		private readonly Grid.Proportion _shortcutProportion;

		private readonly Image _image;
		private readonly TextBlock _textBlock;
		private readonly Menu _subMenu = new VerticalMenu();
		private char? _underscoreChar;
		private string _text;

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

				_textBlock.Text = text;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public SpriteFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		public Color TextColor
		{
			get { return _textBlock.TextColor; }
			set { _textBlock.TextColor = value; }
		}

		internal char? UnderscoreChar
		{
			get
			{
				return _underscoreChar;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public IRenderable Image
		{
			get { return _image.Renderable; }
			set { _image.Renderable = value; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		public IRenderable OpenBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Menu SubMenu
		{
			get { return _subMenu; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		public bool CanOpen
		{
			get
			{
				return _subMenu != null && SubMenu.Items != null && SubMenu.Items.Count > 0;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get { return base.ContentHorizontalAlignment; }
			set { base.ContentHorizontalAlignment = value; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get { return base.ContentVerticalAlignment; }
			set { base.ContentVerticalAlignment = value; }
		}

		[HiddenInEditor]
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

		internal MenuItemButton(Menu menu, MenuItemStyle style)
		{
			Menu = menu;
			InternalChild = new Grid();

			_imageProportion = new Grid.Proportion();
			InternalChild.ColumnsProportions.Add(_imageProportion);
			var textProportion = new Grid.Proportion();
			InternalChild.ColumnsProportions.Add(textProportion);
			_shortcutProportion = new Grid.Proportion();
			InternalChild.ColumnsProportions.Add(_shortcutProportion);

			_image = new Image();
			InternalChild.Widgets.Add(_image);

			_textBlock = new TextBlock
			{
				GridColumn = 1,
			};

			InternalChild.Widgets.Add(_textBlock);

			if (style != null)
			{
				ApplyMenuItemStyle(style);
			}
		}

		public void ApplyMenuItemStyle(MenuItemStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.IconWidth.HasValue)
			{
				_imageProportion.Type = Grid.ProportionType.Pixels;
				_imageProportion.Value = style.IconWidth.Value;
			}

			if (style.ShortcutWidth.HasValue)
			{
				_shortcutProportion.Type = Grid.ProportionType.Pixels;
				_shortcutProportion.Value = style.ShortcutWidth.Value;
			}

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyTextBlockStyle(style.LabelStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();
			_textBlock.IsPressed = IsPressed;
		}

		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			// Only one menu button can be hovered at time
			foreach (var item in Menu.Items)
			{
				if (item.Widget == this)
				{
					continue;
				}

				item.Widget.IsMouseOver = false;
			}
		}
	}
}