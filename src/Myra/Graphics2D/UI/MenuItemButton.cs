using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;
using System;

namespace Myra.Graphics2D.UI
{
	internal class MenuItemButton : ButtonBase<Grid>
	{
		private readonly Grid.Proportion _imageProportion;
		private readonly Grid.Proportion _shortcutProportion;

		private readonly Image _image;
		private readonly TextBlock _textBlock;
		private readonly Menu _subMenu = new VerticalMenu();

		public string Text
		{
			get { return _textBlock.Text; }
			set { _textBlock.Text = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
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
				return _textBlock.UnderscoreChar;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public IRenderable Image
		{
			get { return _image.Renderable; }
			set { _image.Renderable = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[Obsolete("Use Image")]
		public IRenderable TextureRegion
		{
			get { return Image; }
			set { Image = value; }
		}


		[HiddenInEditor]
		[JsonIgnore]
		public IRenderable OpenBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Menu SubMenu
		{
			get { return _subMenu; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public bool CanOpen
		{
			get
			{
				return _subMenu != null && SubMenu.Items != null && SubMenu.Items.Count > 0;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get { return base.ContentHorizontalAlignment; }
			set { base.ContentHorizontalAlignment = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get { return base.ContentVerticalAlignment; }
			set { base.ContentVerticalAlignment = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override bool Toggleable
		{
			get { return base.Toggleable; }

			set
			{
				base.Toggleable = value;
			}
		}

		internal MenuItemButton(MenuItemStyle style)
		{
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
				IsMenuText = true
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
	}
}