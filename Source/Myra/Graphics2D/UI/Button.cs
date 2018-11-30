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
		private IRenderable _textureRegion, _overTextureRegion, _pressedTextureRegion;
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
		public IRenderable Image
		{
			get { return _textureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_textureRegion = value;
				UpdateRenderable();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable OverImage
		{
			get { return _overTextureRegion; }

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_overTextureRegion = value;
				UpdateRenderable();
			}
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public IRenderable PressedImage
		{
			get { return _pressedTextureRegion; }

			set
			{
				if (value == _pressedTextureRegion)
				{
					return;
				}

				_pressedTextureRegion = value;
				UpdateRenderable();
			}
		}

		[EditCategory("Appearance")]
		[DefaultValue(null)]
		public virtual int? ImageWidthHint
		{
			get { return _image.WidthHint; }
			set { _image.WidthHint = value; }
		}

		[EditCategory("Appearance")]
		[DefaultValue(null)]
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

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get => base.ContentHorizontalAlignment;
			set => base.ContentHorizontalAlignment = value;
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get => base.ContentVerticalAlignment;
			set => base.ContentVerticalAlignment = value;
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
				HorizontalAlignment = HorizontalAlignment.Stretch,
				GridPositionX = 1,
				Wrap = true
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

			UpdateRenderable();
		}

		private void UpdateRenderable()
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

			_image.Renderable = image;
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			UpdateRenderable();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateRenderable();
		}

		protected override void FireUp()
		{
			base.FireUp();

			UpdateRenderable();

			_textBlock.IsPressed = IsPressed;
		}

		protected override void FireDown()
		{
			base.FireDown();

			UpdateRenderable();

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