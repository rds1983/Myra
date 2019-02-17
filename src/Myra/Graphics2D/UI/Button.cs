using System;
using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
		private readonly Image _image;
		private readonly TextBlock _textBlock;

		[EditCategory("Appearance")]
		public virtual string Text
		{
			get { return _textBlock.Text; }
			set { _textBlock.Text = value; }
		}

		[EditCategory("Appearance")]
		[StylePropertyPath("LabelStyle.TextColor")]
		public virtual Color TextColor
		{
			get { return _textBlock.TextColor; }
			set { _textBlock.TextColor = value; }
		}

		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable Image
		{
			get { return _image.Renderable; }

			set
			{
				_image.Renderable = value;
			}
		}

		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable OverImage
		{
			get { return _image.OverRenderable; }

			set
			{
				_image.OverRenderable = value;
			}
		}

		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable PressedImage
		{
			get { return _image.PressedRenderable; }

			set
			{
				_image.PressedRenderable = value;
			}
		}

		[EditCategory("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageWidth
		{
			get { return _image.Width; }
			set { _image.Width = value; }
		}

		[Obsolete("Use ImageWidth instead")]
		[HiddenInEditor]
		public int? ImageWidthHint
		{
			get
			{
				return ImageWidth;
			}

			set
			{
				ImageWidth = value;
			}
		}

		[EditCategory("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageHeight
		{
			get { return _image.Height; }
			set { _image.Height = value; }
		}

		[Obsolete("Use ImageHeight instead")]
		[HiddenInEditor]
		public int? ImageHeightHint
		{
			get
			{
				return ImageHeight;
			}

			set
			{
				ImageHeight = value;
			}
		}

		[EditCategory("Appearance")]
		[DefaultValue(true)]
		public virtual bool ImageVisible
		{
			get { return _image.Visible; }

			set { _image.Visible = value; }
		}

		[EditCategory("Appearance")]
		[DefaultValue(0)]
		public int ImageTextSpacing
		{
			get { return InternalChild.ColumnSpacing; }

			set { InternalChild.ColumnSpacing = value; }
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

		public Button(ButtonStyle style)
		{
			InternalChild = new Grid();

			InternalChild.ColumnsProportions.Add(new Grid.Proportion());
			InternalChild.ColumnsProportions.Add(new Grid.Proportion());

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			InternalChild.Widgets.Add(_image);

			_textBlock = new TextBlock
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				GridColumn = 1,
				Wrap = true
			};

			InternalChild.Widgets.Add(_textBlock);

			if (style != null)
			{
				ApplyButtonStyle(style);
			}
		}

		public Button(string style) : this(Stylesheet.Current.ButtonStyles[style])
		{
		}

		public Button() : this(Stylesheet.Current.ButtonStyle)
		{
		}

		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.LabelStyle != null)
			{
				_textBlock.ApplyTextBlockStyle(style.LabelStyle);
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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ButtonStyles.Keys.ToArray();
		}
	}
}