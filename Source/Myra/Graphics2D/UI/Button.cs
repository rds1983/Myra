using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Button : ButtonBase<Grid>
	{
		private Drawable _textureRegion, _overTextureRegion, _pressedTextureRegion;
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
		public Drawable Image
		{
			get { return _textureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_textureRegion = value;
				UpdateDrawable();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable OverImage
		{
			get { return _overTextureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_overTextureRegion = value;
				UpdateDrawable();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public Drawable PressedImage
		{
			get { return _pressedTextureRegion; }

			set
			{
				if (value == _pressedTextureRegion)
				{
					return;
				}

				_pressedTextureRegion = value;
				UpdateDrawable();
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
			Widget = new Grid((GridStyle)null);

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
				GridPositionX = 1
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

			UpdateDrawable();
		}

		private void UpdateDrawable()
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

			_image.Drawable = image;
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			UpdateDrawable();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateDrawable();
		}

		protected override void FireUp()
		{
			base.FireUp();

			UpdateDrawable();

			_textBlock.IsPressed = IsPressed;
		}

		protected override void FireDown()
		{
			base.FireDown();

			UpdateDrawable();

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