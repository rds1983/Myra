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
		private readonly Proportion _imageProportion;
		private readonly Proportion _shortcutProportion;

		private readonly Image _image;
		private readonly Label _textBlock;
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

		[Browsable(false)]
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

		internal MenuItemButton(Menu menu, MenuItemStyle style)
		{
			Menu = menu;
			InternalChild = new HorizontalStackPanel();

			_imageProportion = new Proportion();
			InternalChild.Proportions.Add(_imageProportion);
			var textProportion = new Proportion();
			InternalChild.Proportions.Add(textProportion);
			_shortcutProportion = new Proportion();
			InternalChild.Proportions.Add(_shortcutProportion);

			_image = new Image();
			InternalChild.Widgets.Add(_image);

			_textBlock = new Label
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
			ApplyButtonStyle(style);

			if (style.IconWidth.HasValue)
			{
				_imageProportion.Type = ProportionType.Pixels;
				_imageProportion.Value = style.IconWidth.Value;
			}

			if (style.ShortcutWidth.HasValue)
			{
				_shortcutProportion.Type = ProportionType.Pixels;
				_shortcutProportion.Value = style.ShortcutWidth.Value;
			}

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyLabelStyle(style.LabelStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();
			_textBlock.IsPressed = IsPressed;
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
	}
}