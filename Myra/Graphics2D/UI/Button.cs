using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
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

		public Drawable Image { get; set; }
		public Drawable PressedImage { get; set; }

		public bool ImageVisible
		{
			get { return _image.Visible; }

			set { _image.Visible = value; }
		}

		public Button(ButtonStyle style)
		{
			Widget = new Grid();

			_image = new Image();
			Widget.Children.Add(_image);

			_textBlock = new TextBlock {GridPosition = {X = 1}};

			Widget.Children.Add(_textBlock);

			if (style != null)
			{
				ApplyButtonStyle(style);
			}
		}

		public Button() : this(Stylesheet.Current.ButtonStyle)
		{
		}

		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyTextBlockStyle(style.LabelStyle);
			}

			Image = style.Image;
			PressedImage = style.PressedImage;
			UpdateDrawable();
		}

		private void UpdateDrawable()
		{
			_image.Drawable = IsPressed ? PressedImage : Image;
		}

		protected override void FireUp()
		{
			base.FireUp();

			UpdateDrawable();
		}

		protected override void FireDown()
		{
			base.FireDown();

			UpdateDrawable();
		}
	}
}