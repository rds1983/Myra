using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ImageButton: ButtonBase<Image>
	{
		private Drawable _image, _overImage, _pressedImage;

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable Image
		{
			get
			{
				return _image;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_image = value;
				UpdateTextureRegion();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable OverImage
		{
			get
			{
				return _overImage;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_overImage = value;
				UpdateTextureRegion();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable PressedImage
		{
			get
			{
				return _pressedImage;
			}

			set
			{
				if (value == _pressedImage)
				{
					return;
				}

				_pressedImage = value;
				UpdateTextureRegion();
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
			: this(Stylesheet.Current.ImageButtonStyles[style])
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

			UpdateTextureRegion();
		}

		private void UpdateTextureRegion()
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

			Widget.Drawable = image;
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			UpdateTextureRegion();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateTextureRegion();
		}

		protected override void FireUp()
		{
			base.FireUp();

			UpdateTextureRegion();
		}

		protected override void FireDown()
		{
			base.FireDown();

			UpdateTextureRegion();
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyImageButtonStyle(stylesheet.ImageButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ImageButtonStyles.Keys.ToArray();
		}
	}
}