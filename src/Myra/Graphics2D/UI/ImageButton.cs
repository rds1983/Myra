using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class ImageButton: ButtonBase<Image>
	{
		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
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
		[HiddenInEditor]
		[EditCategory("Appearance")]
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
		[HiddenInEditor]
		[EditCategory("Appearance")]
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

		public ImageButton(string style): this(new ImageButtonStyle(Stylesheet.Current.ButtonStyles[style]))
		{
		}

		public ImageButton() : this(new ImageButtonStyle(Stylesheet.Current.ButtonStyle))
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