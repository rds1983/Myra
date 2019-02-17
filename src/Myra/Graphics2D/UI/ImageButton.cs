using System.Linq;
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
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
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