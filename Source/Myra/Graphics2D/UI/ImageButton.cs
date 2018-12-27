using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ImageButton: ButtonBase<Image>
	{
		private IRenderable _image, _overImage, _pressedImage;

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable Image
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
		public IRenderable OverImage
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
		public IRenderable PressedImage
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
		[DefaultValue(null)]
		public virtual int? ImageWidthHint
		{
			get { return InternalChild.Width; }
			set { InternalChild.Width = value; }
		}

		[EditCategory("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageHeightHint
		{
			get { return InternalChild.Height; }
			set { InternalChild.Height = value; }
		}

		public ImageButton(ImageButtonStyle style)
		{
			InternalChild = new Image
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

			var imageStyle = style.ImageStyle;

			InternalChild.ApplyWidgetStyle(imageStyle);

			Image = imageStyle.Image;
			OverImage = imageStyle.OverImage;
			PressedImage = imageStyle.PressedImage;

			InternalChild.Width = null;
			InternalChild.Height = null;
			InternalChild.UpdateImageSize(imageStyle.Image);
			InternalChild.UpdateImageSize(imageStyle.OverImage);
			InternalChild.UpdateImageSize(imageStyle.PressedImage);

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

			InternalChild.Renderable = image;
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