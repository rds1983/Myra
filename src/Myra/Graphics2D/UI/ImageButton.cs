using System.Xml.Serialization;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class ImageButton : ButtonBase<Image>
	{
		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IBrush Image
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

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IBrush OverImage
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

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IBrush PressedImage
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