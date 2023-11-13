using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System;
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

	[StyleTypeName("CheckBox")]
	public class CheckButton : ButtonBase2
	{
		private class CheckImageInternal : Image
		{
			protected override bool UseOverBackground
			{
				get
				{
					if (Parent == null)
					{
						return base.IsMouseInside;
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

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool IsChecked
		{
			get => IsPressed;
			set => IsPressed = value;
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

		public event EventHandler IsCheckedChanged
		{
			add
			{
				PressedChanged += value;
			}

			remove
			{
				PressedChanged -= value;
			}
		}

		public CheckButton(string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			SetStyle(styleName);

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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			base.InternalSetStyle(stylesheet, name);

			var style = stylesheet.CheckBoxStyles.SafelyGetStyle(name);
			ApplyCheckButtonStyle(style);
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
	}
}
