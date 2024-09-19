using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	[StyleTypeName("Button")]
	public class ToggleButton : ButtonBase2
	{
		private readonly SingleItemLayout<Widget> _layout;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool IsToggled
		{
			get => IsPressed;
			set => IsPressed = value;
		}


		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get => _layout.Child;
			set => _layout.Child = value;
		}

		public event EventHandler IsToggledChanged
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

		public ToggleButton(string styleName = Stylesheet.DefaultStyleName)
		{
			_layout = new SingleItemLayout<Widget>(this);
			ChildrenLayout = _layout;
			SetStyle(styleName);
		}

		protected override void InternalOnTouchUp()
		{
		}

		protected override void InternalOnTouchDown()
		{
			SetValueByUser(!IsPressed);
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
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}
