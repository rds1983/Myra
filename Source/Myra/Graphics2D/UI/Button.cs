using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
		private TextureRegion2D _TextureRegion2D, _overTextureRegion2D, _pressedTextureRegion2D;
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
		public TextureRegion2D Image
		{
			get
			{
				return _TextureRegion2D;
			}

			set
			{
				if (value == _TextureRegion2D)
				{
					return;
				}

				_TextureRegion2D = value;
				UpdateTextureRegion2D();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion2D OverImage
		{
			get
			{
				return _overTextureRegion2D;
			}

			set
			{
				if (value == _TextureRegion2D)
				{
					return;
				}

				_overTextureRegion2D = value;
				UpdateTextureRegion2D();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion2D PressedImage
		{
			get
			{
				return _pressedTextureRegion2D;
			}

			set
			{
				if (value == _pressedTextureRegion2D)
				{
					return;
				}

				_pressedTextureRegion2D = value;
				UpdateTextureRegion2D();
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

			if (style.ImageStyle != null)
			{
				var imageStyle = style.ImageStyle;

				_image.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				OverImage = imageStyle.OverImage;
				PressedImage = imageStyle.PressedImage;

				_image.UpdateImageSize(imageStyle.Image);
				_image.UpdateImageSize(imageStyle.OverImage);
				_image.UpdateImageSize(imageStyle.PressedImage);
			}

			UpdateTextureRegion2D();
		}

		private void UpdateTextureRegion2D()
		{
			var image = Image;
			if (IsPressed && PressedImage != null)
			{
				image = PressedImage;
			}
			else if (IsMouseOver && OverImage != null)
			{
				image = OverImage;
			}

			_image.TextureRegion2D = image;
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			UpdateTextureRegion2D();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateTextureRegion2D();
		}

		protected override void FireUp()
		{
			base.FireUp();

			UpdateTextureRegion2D();
		}

		protected override void FireDown()
		{
			base.FireDown();

			UpdateTextureRegion2D();
		}
	}
}