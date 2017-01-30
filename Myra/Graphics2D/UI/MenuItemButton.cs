using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class MenuItemButton : ButtonBase<Grid>
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
		public BitmapFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		public Color TextColor
		{
			get { return _textBlock.TextColor; }
			set { _textBlock.TextColor = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Drawable Drawable
		{
			get { return _image.Drawable; }
			set { _image.Drawable = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Drawable OpenBackground { get; set; }

		[HiddenInEditor]
		public Menu SubMenu
		{
			get { return _subMenu; }
		}


		[HiddenInEditor]
		[JsonIgnore]
		public bool IsSubMenuOpen { get; internal set; }

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
			IsSubMenuOpen = false;

			Widget = new Grid();

			_imageProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(_imageProportion);
			var textProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(textProportion);
			_shortcutProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(_shortcutProportion);

			_image = new Image();
			Widget.Widgets.Add(_image);

			_textBlock = new TextBlock
			{
				GridPosition = {X = 1}
			};

			Widget.Widgets.Add(_textBlock);

			if (style != null)
			{
				ApplyMenuItemStyle(style);
			}
		}

		public override Drawable GetCurrentBackground()
		{
			if (IsSubMenuOpen && OpenBackground != null)
			{
				return OpenBackground;
			}

			return base.GetCurrentBackground();
		}

		public void ApplyMenuItemStyle(MenuItemStyle style)
		{
			ApplyButtonBaseStyle(style);

			OpenBackground = style.OpenBackground;

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
	}
}