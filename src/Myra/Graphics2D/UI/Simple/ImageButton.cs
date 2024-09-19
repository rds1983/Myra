using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
	public class ImageButton : ButtonBase<Image>
	{
		[Category("Appearance")]
		public IImage Image
		{
			get
			{
				return InternalChild.Renderable;
			}

			set
			{
				InternalChild.Renderable = value;
			}
		}

		[Category("Appearance")]
		public IImage OverImage
		{
			get
			{
				return InternalChild.OverRenderable;
			}

			set
			{
				InternalChild.OverRenderable = value;
			}
		}

		[Category("Appearance")]
		public IImage PressedImage
		{
			get
			{
				return InternalChild.PressedRenderable;
			}

			set
			{
				InternalChild.PressedRenderable = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public int? ImageWidth
		{
			get
			{
				return InternalChild.Width;
			}
			set
			{
				InternalChild.Width = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public int? ImageHeight
		{
			get
			{
				return InternalChild.Height;
			}
			set
			{
				InternalChild.Height = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public HorizontalAlignment ImageHorizontalAlignment
		{
			get => InternalChild.HorizontalAlignment;
			set => InternalChild.HorizontalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment ImageVerticalAlignment
		{
			get => InternalChild.VerticalAlignment;
			set => InternalChild.VerticalAlignment = value;
		}

		public ImageButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			SetStyle(styleName);
		}

		public void ApplyImageButtonStyle(ImageButtonStyle style)
		{
			ApplyButtonStyle(style);

			var imageStyle = style.ImageStyle;

			if (imageStyle != null)
			{
				InternalChild.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				OverImage = imageStyle.OverImage;
				PressedImage = imageStyle.PressedImage;
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}
	}
}