using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ImageButton: ButtonBase<Image>
	{
		private Drawable _drawable, _overDrawable, _pressedDrawable;

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
		public Drawable OverImage
		{
			get
			{
				return _overDrawable;
			}

			set
			{
				if (value == _drawable)
				{
					return;
				}

				_overDrawable = value;
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

		public ImageButton() : this(DefaultAssets.UIStylesheet.ImageButtonStyle)
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

			UpdateDrawable();
		}

		private void UpdateDrawable()
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

			UpdateDrawable();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateDrawable();
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
