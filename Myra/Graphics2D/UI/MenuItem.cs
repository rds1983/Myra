using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class MenuItem : ButtonBase<Grid>
	{
		private readonly Grid.Proportion _imageProportion;
		private readonly Grid.Proportion _shortcutProportion;

		private readonly Image _image;
		private readonly TextBlock _textBlock;

		public string Text
		{
			get { return _textBlock.Text; }
			set { _textBlock.Text = value; }
		}

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

		public Drawable Drawable
		{
			get { return _image.Drawable; }
			set { _image.Drawable = value; }
		}

		public Menu SubMenu { get; set; }

		public MenuItem()
		{
			Widget = new Grid();

			_imageProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(_imageProportion);
			var textProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(textProportion);
			_shortcutProportion = new Grid.Proportion();
			Widget.ColumnsProportions.Add(_shortcutProportion);

			_image = new Image();
			Widget.Children.Add(_image);

			_textBlock = new TextBlock
			{
				GridPosition = {X = 1}
			};

			Widget.Children.Add(_textBlock);
		}

		public void AddMenuItem(MenuItem menuItem)
		{
			if (SubMenu == null)
			{
				SubMenu = new Menu(Orientation.Vertical);
			}

			SubMenu.AddMenuItem(menuItem);
		}

		public void AddSeparator()
		{
			if (SubMenu == null)
			{
				SubMenu = new Menu(Orientation.Vertical);
			}

			SubMenu.AddSeparator();
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
	}
}