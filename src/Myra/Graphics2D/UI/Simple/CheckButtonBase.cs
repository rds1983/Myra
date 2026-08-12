using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;


#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the position of the check box image relative to the content in a check button.
	/// </summary>
	public enum CheckPosition
	{
		/// <summary>The check box image is positioned to the left of the content.</summary>
		Left,
		/// <summary>The check box image is positioned to the right of the content.</summary>
		Right
	}

	/// <summary>
	/// An abstract base class for check button widgets that display a checkbox image and content with toggle functionality.
	/// </summary>
	public class CheckButtonBase : ButtonBase
	{
		private class CheckImageInternal : Image
		{
			protected override bool UseOverBackground
			{
				get
				{
					if (Parent == null)
					{
						return IsMouseInside;
					}

					return Parent.IsMouseInside;
				}
			}
		}


		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Horizontal);
		private CheckPosition _checkPosition = CheckPosition.Left;
		private readonly CheckImageInternal _check = new CheckImageInternal
		{
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Center,
		};
		private Widget _content;
		private IImage _checkedImage, _uncheckedImage;

		/// <summary>
		/// Gets or sets the position of the check box image relative to the content.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(CheckPosition.Left)]
		public CheckPosition CheckPosition
		{
			get => _checkPosition;
			set
			{
				if (_checkPosition == value) return;

				_checkPosition = value;
				UpdateChildren();
			}
		}

		/// <summary>
		/// Gets or sets the spacing in pixels between the check box image and the content.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0)]
		[StylePropertyPath("ImageTextSpacing")]
		public int CheckContentSpacing
		{
			get => _layout.Spacing;
			set => _layout.Spacing = value;
		}

		/// <summary>
		/// Gets or sets the image displayed when the check button is unchecked.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("ImageStyle/Image")]
		public IImage UncheckedImage
		{
			get => _uncheckedImage;
			set
			{
				if (value == _uncheckedImage)
				{
					return;
				}

				_uncheckedImage = value;
				UpdateImage();
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when the check button is checked.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("ImageStyle/PressedImage")]
		public IImage CheckedImage
		{
			get => _checkedImage;
			set
			{
				if (value == _checkedImage)
				{
					return;
				}

				_checkedImage = value;
				UpdateImage();
			}
		}

		/// <summary>
		/// Gets or sets the content widget displayed next to the check box.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get => _content;
			set
			{
				if (_content == value) return;

				_content = value;

				UpdateChildren();
			}
		}

		/// <summary>
		/// Gets the image widget that displays the check box visual.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Image CheckImage => _check;

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckButtonBase"/> class.
		/// </summary>
		protected CheckButtonBase()
		{
			ChildrenLayout = _layout;

			UpdateChildren();
		}

		/// <summary>
		/// Called when a touch point is released on the check button base.
		/// </summary>
		protected override void InternalOnTouchUp()
		{
		}

		/// <summary>
		/// Called when a touch point is pressed on the check button base, toggling its state.
		/// </summary>
		protected override void InternalOnTouchDown()
		{
			SetIsPressedByUser(!IsPressed);
		}

		/// <summary>
		/// Handles the pressed state change and updates the check image.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_check.IsPressed = IsPressed;
		}

		/// <summary>
		/// Handles keyboard input, toggling the check state when Space is pressed.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!Enabled)
			{
				return;
			}

			if (k == Keys.Space)
			{
				SetIsPressedByUser(!IsPressed);
			}
		}

		/// <summary>
		/// Applies the specified widget style to this check button.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var checkButtonStyle = (CheckButtonStyle)style;
			if (checkButtonStyle.ImageStyle != null)
			{
				_check.ApplyImageStyle(checkButtonStyle.ImageStyle);

				UncheckedImage = checkButtonStyle.ImageStyle.Image;
				CheckedImage = checkButtonStyle.ImageStyle.PressedImage;
			}

			CheckContentSpacing = checkButtonStyle.ImageTextSpacing;
		}

		private void UpdateChildren()
		{
			Children.Clear();

			switch (_checkPosition)
			{
				case CheckPosition.Left:
					Children.Add(_check);
					if (_content != null)
					{
						Children.Add(_content);
					}

					break;

				case CheckPosition.Right:
					if (_content != null)
					{
						Children.Add(_content);
					}
					Children.Add(_check);

					break;
			}
		}

		/// <summary>
		/// Copies the check button properties from another check button.
		/// </summary>
		/// <param name="w">The source check button to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var checkButtonBase = (CheckButtonBase)w;

			CheckPosition = checkButtonBase.CheckPosition;
			CheckContentSpacing = checkButtonBase.CheckContentSpacing;
			CheckImage.CopyFrom(checkButtonBase.CheckImage);
		}

		private void UpdateImage()
		{
			_check.Renderable = IsPressed ? _checkedImage : _uncheckedImage;
		}
	}
}
