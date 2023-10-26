using Myra.Graphics2D.UI.Styles;
using Myra.Attributes;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	[StyleTypeName("Button")]
	public class Button : ButtonBase2
	{
		internal bool ReleaseOnTouchLeft;

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				// If we're not releasing the button on touch left,
				// we have to do it on touch up
				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		public Button(string styleName = Stylesheet.DefaultStyleName)
		{
			ReleaseOnTouchLeft = true;

			SetStyle(styleName);
		}

		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			if (ReleaseOnTouchLeft)
			{
				SetValueByUser(false);
			}
		}

		protected override void InternalOnTouchUp()
		{
			SetValueByUser(false);
		}

		protected override void InternalOnTouchDown()
		{
			SetValueByUser(true);
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
				// Emulate click
				DoClick();
			}
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			IsPressed = false;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}

		public static Button CreateTextButton(string text)
		{
			return new Button
			{
				Content = new Label
				{
					Text = text
				}
			};
		}
	}
}