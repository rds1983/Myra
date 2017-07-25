using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ImageButton: ButtonBase<Image>
	{
		private TextureRegion2D _textureRegion2D, _overTextureRegion2D, _pressedTextureRegion2D;

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion2D Image
		{
			get
			{
				return _textureRegion2D;
			}

			set
			{
				if (value == _textureRegion2D)
				{
					return;
				}

				_textureRegion2D = value;
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
				if (value == _textureRegion2D)
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
			get { return Widget.WidthHint; }
			set { Widget.WidthHint = value; }
		}

		[EditCategory("Appearance")]
		public virtual int? ImageHeightHint
		{
			get { return Widget.HeightHint; }
			set { Widget.HeightHint = value; }
		}

		[EditCategory("Appearance")]
		public virtual bool ImageVisible
		{
			get { return Widget.Visible; }

			set { Widget.Visible = value; }
		}

		public ImageButton(ImageButtonStyle style)
		{
			Widget = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			if (style != null)
			{
				ApplyImageButtonStyle(style);
			}
		}

		public ImageButton(string style)
			: this(Stylesheet.Current.ImageButtonVariants[style])
		{
		}

		public ImageButton() : this(Stylesheet.Current.ImageButtonStyle)
		{
		}

		public void ApplyImageButtonStyle(ImageButtonStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.ImageStyle != null)
			{
				var imageStyle = style.ImageStyle;

				Widget.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				OverImage = imageStyle.OverImage;
				PressedImage = imageStyle.PressedImage;

				Widget.UpdateImageSize(imageStyle.Image);
				Widget.UpdateImageSize(imageStyle.OverImage);
				Widget.UpdateImageSize(imageStyle.PressedImage);
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

			Widget.TextureRegion2D = image;
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
