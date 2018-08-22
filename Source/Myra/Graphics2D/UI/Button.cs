using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
		private TextureRegion _textureRegion, _overTextureRegion, _pressedTextureRegion;
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

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion Image
		{
			get { return _textureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_textureRegion = value;
				UpdateTextureRegion();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Color ImageColor
		{
			get
			{
				return _image.Color;
			}

			set
			{
				_image.Color = value;
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion OverImage
		{
			get { return _overTextureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_overTextureRegion = value;
				UpdateTextureRegion();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public TextureRegion PressedImage
		{
			get { return _pressedTextureRegion; }

			set
			{
				if (value == _pressedTextureRegion)
				{
					return;
				}

				_pressedTextureRegion = value;
				UpdateTextureRegion();
			}
		}

		[EditCategory("Appearance")]
		public virtual int? ImageWidthHint
		{
			get { return _image.WidthHint; }
			set { _image.WidthHint = value; }
		}

		[EditCategory("Appearance")]
		public virtual int? ImageHeightHint
		{
			get { return _image.HeightHint; }
			set { _image.HeightHint = value; }
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
			get { return Widget.ColumnSpacing; }

			set { Widget.ColumnSpacing = value; }
		}

		public Button(ButtonStyle style)
		{
			Widget = new Grid();

			Widget.ColumnsProportions.Add(new Grid.Proportion());
			Widget.ColumnsProportions.Add(new Grid.Proportion());

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Widget.Widgets.Add(_image);

			_textBlock = new TextBlock
			{
				GridPositionX = 1,
				Wrap = false
			};

			Widget.Widgets.Add(_textBlock);

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

				_image.UpdateImageSize(imageStyle.Image);
				_image.UpdateImageSize(imageStyle.OverImage);
				_image.UpdateImageSize(imageStyle.PressedImage);
			}

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

			_image.TextureRegion = image;
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
			ApplyButtonStyle(stylesheet.ButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ButtonStyles.Keys.ToArray();
		}
	}
}