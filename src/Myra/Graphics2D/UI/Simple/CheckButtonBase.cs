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
	public enum CheckPosition
	{
		Left,
		Right
	}

	public class CheckButtonBase : ButtonBase2
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

		[Category("Behavior")]
		[DefaultValue(0)]
		public int CheckContentSpacing
		{
			get => _layout.Spacing;
			set => _layout.Spacing = value;
		}

		[Category("Appearance")]
		public IImage UncheckedImage
		{
			get => _check.Renderable;
			set => _check.Renderable = value;
		}

		[Category("Appearance")]
		public IImage CheckedImage
		{
			get => _check.PressedRenderable;
			set => _check.PressedRenderable = value;
		}

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

		[Browsable(false)]
		[XmlIgnore]
		public Image CheckImage => _check;

		protected CheckButtonBase()
		{
			ChildrenLayout = _layout;

			UpdateChildren();
		}

		protected override void InternalOnTouchUp()
		{
		}

		protected override void InternalOnTouchDown()
		{
			SetValueByUser(!IsPressed);
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_check.IsPressed = IsPressed;
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!Enabled)
			{
				return;
			}

			if (k == Keys.Space)
			{
				SetValueByUser(!IsPressed);
			}
		}

		public void ApplyCheckButtonStyle(ImageTextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				_check.ApplyPressableImageStyle(style.ImageStyle);
			}

			CheckContentSpacing = style.ImageTextSpacing;
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

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var checkButtonBase = (CheckButtonBase)w;

			CheckPosition = checkButtonBase.CheckPosition;
			CheckContentSpacing = checkButtonBase.CheckContentSpacing;
			CheckImage.CopyFrom(checkButtonBase.CheckImage);
		}
	}
}
