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
		[DefaultValue(null)]
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

		[Category("Appearance")]
		public IImage Image
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

		[Category("Appearance")]
		public IImage OverImage
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

		[Category("Appearance")]
		public IImage PressedImage
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

		public ImageTextButton(string styleName = Stylesheet.DefaultStyleName)
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

			_textBlock = new Label(null)
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center,
				Wrap = true
			};

			InternalChild.Widgets.Add(_textBlock);

			SetStyle(styleName);
		}

		public void ApplyImageTextButtonStyle(ImageTextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				_image.ApplyPressableImageStyle(style.ImageStyle);
			}

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyLabelStyle(style.LabelStyle);
			}

			ImageTextSpacing = style.ImageTextSpacing;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_image.IsPressed = IsPressed;
			_textBlock.IsPressed = IsPressed;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(new ImageTextButtonStyle(stylesheet.ButtonStyles[name], stylesheet.LabelStyle));
		}
	}
}