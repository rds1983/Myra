using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
		private Drawable _drawable, _pressedDrawable;
		private readonly Image _image;
		private readonly TextBlock _textBlock;

		[EditCategory("Appearance")]
		public virtual string Text
		{
			get { return _textBlock.Text; }
			set { _textBlock.Text = value; }
		}

		[EditCategory("Appearance")]
		public virtual Color TextColor
		{
			get { return _textBlock.TextColor; }
			set { _textBlock.TextColor = value; }
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public BitmapFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable Image
		{
			get
			{
				return _drawable;
			}

			set
			{
				if (value == _drawable)
				{
					return;
				}

				_drawable = value;
				UpdateDrawable();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable PressedImage
		{
			get
			{
				return _pressedDrawable;
			}

			set
			{
				if (value == _pressedDrawable)
				{
					return;
				}

				_pressedDrawable = value;
				UpdateDrawable();
			}
		}

		[EditCategory("Appearance")]
		public virtual int? ImageWidthHint
		{
			get { return _image.WidthHint; }
			set { _image.WidthHint = value; }
		}

		[EditCategory("Appearance")]
		public virtual int? ImageHeightHint
		{
			get { return _image.HeightHint; }
			set { _image.HeightHint = value; }
		}

		[EditCategory("Appearance")]
		public virtual bool ImageVisible
		{
			get { return _image.Visible; }

			set { _image.Visible = value; }
		}

		public Button(ButtonStyle style)
		{
			Widget = new Grid();

			Widget.ColumnsProportions.Add(new Grid.Proportion());
			Widget.ColumnsProportions.Add(new Grid.Proportion());

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Widget.Widgets.Add(_image);

			_textBlock = new TextBlock
			{
				GridPositionX = 1
			};

			Widget.Widgets.Add(_textBlock);

			if (style != null)
			{
				ApplyButtonStyle(style);
			}
		}

		public Button() : this(DefaultAssets.UIStylesheet.ButtonStyle)
		{
		}

		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyTextBlockStyle(style.LabelStyle);
			}

			if (style.ImageStyle != null)
			{
				var imageStyle = style.ImageStyle;

				_image.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				PressedImage = imageStyle.PressedImage;

				if (Image != null)
				{
					if (_image.WidthHint == null || 
						imageStyle.Image.Size.X > _image.WidthHint.Value)
					{
						_image.WidthHint = imageStyle.Image.Size.X;
					}

					if (_image.HeightHint == null ||
						imageStyle.Image.Size.Y > _image.HeightHint.Value)
					{
						_image.HeightHint = imageStyle.Image.Size.Y;
					}
				}

				if (imageStyle.PressedImage != null)
				{
					if (_image.WidthHint == null ||
						imageStyle.PressedImage.Size.X > _image.WidthHint.Value)
					{
						_image.WidthHint = imageStyle.PressedImage.Size.X;
					}

					if (_image.HeightHint == null ||
						imageStyle.PressedImage.Size.Y > _image.HeightHint.Value)
					{
						_image.HeightHint = imageStyle.PressedImage.Size.Y;
					}
				}
			}

			UpdateDrawable();
		}

		private void UpdateDrawable()
		{
			var image = Image;
			if (IsPressed && PressedImage != null)
			{
				image = PressedImage;
			}

			_image.Drawable = image;
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