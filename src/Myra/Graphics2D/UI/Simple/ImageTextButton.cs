using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Attributes;
using FontStashSharp;
using System;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{

	[Obsolete("Use Button")]
	[StyleTypeName("Button")]
	public class ImageTextButton : ButtonBase<Grid>
	{
		public enum TextPositionEnum
		{
			Right,
			Left,
			Top,
			Bottom,
			OverlapsImage,
			BehindImage
		}

		private readonly Image _image;
		private readonly Label _label;
		private TextPositionEnum _textPosition;

		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _label.Text;
			}
			set
			{
				_label.Text = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/TextColor")]
		public Color TextColor
		{
			get
			{
				return _label.TextColor;
			}
			set
			{
				_label.TextColor = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/OverTextColor")]
		public Color? OverTextColor
		{
			get
			{
				return _label.OverTextColor;
			}
			set
			{
				_label.OverTextColor = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/PressedTextColor")]
		public Color? PressedTextColor
		{
			get
			{
				return _label.PressedTextColor;
			}
			set
			{
				_label.PressedTextColor = value;
			}
		}

		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _label.Font;
			}
			set
			{
				_label.Font = value;
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
		public int? ImageWidth
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
		public int? ImageHeight
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
		public bool ImageVisible
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
				return InternalChild.ColumnSpacing;
			}

			set
			{
				InternalChild.ColumnSpacing = value;
				InternalChild.RowSpacing = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(TextPositionEnum.Right)]
		public TextPositionEnum TextPosition
		{
			get
			{
				return _textPosition;
			}

			set
			{
				if (_textPosition == value)
				{
					return;
				}

				SetTextPosition(value);
			}
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment LabelHorizontalAlignment
		{
			get => _label.HorizontalAlignment;
			set => _label.HorizontalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment LabelVerticalAlignment
		{
			get => _label.VerticalAlignment;
			set => _label.VerticalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public HorizontalAlignment ImageHorizontalAlignment
		{
			get => _image.HorizontalAlignment;
			set => _image.HorizontalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment ImageVerticalAlignment
		{
			get => _image.VerticalAlignment;
			set => _image.VerticalAlignment = value;
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
			InternalChild = new Grid
			{
				DefaultColumnProportion = Proportion.Auto,
				DefaultRowProportion = Proportion.Auto
			};

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			_label = new Label(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Wrap = true
			};

			SetStyle(styleName);
			SetTextPosition(TextPositionEnum.Right);
		}

		private void SetTextPosition(TextPositionEnum value)
		{
			InternalChild.Widgets.Clear();
			InternalChild.ColumnsProportions.Clear();

			switch (value)
			{
				case TextPositionEnum.Right:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 1);
					Grid.SetRow(_label, 0);
					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.Left:
					Grid.SetColumn(_image, 1);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.OverlapsImage:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.BehindImage:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_label);
					InternalChild.Widgets.Add(_image);
					break;
				case TextPositionEnum.Top:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 1);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.Bottom:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 1);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
			}

			_textPosition = value;
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
				_label.ApplyLabelStyle(style.LabelStyle);
			}

			ImageTextSpacing = style.ImageTextSpacing;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_image.IsPressed = IsPressed;
			_label.IsPressed = IsPressed;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(new ImageTextButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name)));
		}
	}
}