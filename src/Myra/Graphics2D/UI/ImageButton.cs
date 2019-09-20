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
		public IRenderable Image
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
		public IRenderable OverImage
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
		public IRenderable PressedImage
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

		public ImageButton(Stylesheet stylesheet, string style) :
			this(stylesheet.ButtonStyles[style].ToImageButtonStyle())
		{
		}

		public ImageButton(Stylesheet stylesheet) :
			this(stylesheet.ButtonStyle.ToImageButtonStyle())
		{
		}

		public ImageButton(string style) : this(Stylesheet.Current, style)
		{
		}

		public ImageButton() : this(Stylesheet.Current)
		{
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