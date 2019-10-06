using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Attributes;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ImageTextButton : ButtonBase<HorizontalStackPanel>
	{
		private readonly Image _image;
		private readonly Label _textBlock;

		[Category("Appearance")]
		public virtual string Text
		{
			get
			{
				return _textBlock.Text;
			}
			set
			{
				_textBlock.Text = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/TextColor")]
		public virtual Color TextColor
		{
			get
			{
				return _textBlock.TextColor;
			}
			set
			{
				_textBlock.TextColor = value;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public SpriteFont Font
		{
			get
			{
				return _textBlock.Font;
			}
			set
			{
				_textBlock.Font = value;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IRenderable Image
		{
			get
			{
				return _image.Renderable;
			}

			set
			{
				_image.Renderable = value;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IRenderable OverImage
		{
			get
			{
				return _image.OverRenderable;
			}

			set
			{
				_image.OverRenderable = value;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public IRenderable PressedImage
		{
			get
			{
				return _image.PressedRenderable;
			}

			set
			{
				_image.PressedRenderable = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageWidth
		{
			get
			{
				return _image.Width;
			}
			set
			{
				_image.Width = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageHeight
		{
			get
			{
				return _image.Height;
			}
			set
			{
				_image.Height = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		public virtual bool ImageVisible
		{
			get
			{
				return _image.Visible;
			}

			set
			{
				_image.Visible = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(0)]
		public int ImageTextSpacing
		{
			get
			{
				return InternalChild.Spacing;
			}

			set
			{
				InternalChild.Spacing = value;
			}
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get
			{
				return base.ContentHorizontalAlignment;
			}
			set
			{
				base.ContentHorizontalAlignment = value;
			}
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get
			{
				return base.ContentVerticalAlignment;
			}
			set
			{
				base.ContentVerticalAlignment = value;
			}
		}

		public ImageTextButton(ImageTextButtonStyle style)
		{
			InternalChild = new HorizontalStackPanel
			{
				VerticalAlignment = VerticalAlignment.Stretch
			};

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			InternalChild.Widgets.Add(_image);

			_textBlock = new Label(style != null ? style.LabelStyle : null)
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Wrap = true
			};

			InternalChild.Widgets.Add(_textBlock);

			if (style != null)
			{
				ApplyImageTextButtonStyle(style);
			}
		}

		public ImageTextButton(Stylesheet stylesheet, string style) :
			this(stylesheet.ButtonStyles[style].ToImageTextButtonStyle(stylesheet.LabelStyle))
		{
		}

		public ImageTextButton(Stylesheet stylesheet) :
			this(stylesheet.ButtonStyle.ToImageTextButtonStyle(stylesheet.LabelStyle))
		{
		}

		public ImageTextButton(string style) : this(Stylesheet.Current, style)
		{
		}

		public ImageTextButton() : this(Stylesheet.Current)
		{
		}

		public void ApplyImageTextButtonStyle(ImageTextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyLabelStyle(style.LabelStyle);
			}

			if (style.ImageStyle != null)
			{
				var imageStyle = style.ImageStyle;

				_image.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				OverImage = imageStyle.OverImage;
				PressedImage = imageStyle.PressedImage;
			}

			ImageTextSpacing = style.ImageTextSpacing;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_image.IsPressed = IsPressed;
			_textBlock.IsPressed = IsPressed;
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(new ImageTextButtonStyle(stylesheet.ButtonStyles[name], stylesheet.LabelStyle));
		}
	}
}