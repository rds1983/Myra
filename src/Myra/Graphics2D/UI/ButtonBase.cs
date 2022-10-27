using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isPressed = false;

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public virtual HorizontalAlignment ContentHorizontalAlignment
		{
			get { return InternalChild.HorizontalAlignment; }
			set { InternalChild.HorizontalAlignment = value; }
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public virtual VerticalAlignment ContentVerticalAlignment
		{
			get { return InternalChild.VerticalAlignment; }
			set { InternalChild.VerticalAlignment = value; }
		}

		[Category("Appearance")]
		public virtual IBrush PressedBackground { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool Toggleable { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsPressed
		{
			get
			{
				return _isPressed;
			}

			set
			{
				if (value == _isPressed)
				{
					return;
				}

				_isPressed = value;

				OnPressedChanged();
			}
		}

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

		protected internal override void OnActiveChanged()
		{
			base.OnActiveChanged();

			if (!Active && IsPressed && !Toggleable)
			{
				IsPressed = false;
			}
		}

		public event EventHandler Click;
		public event EventHandler PressedChanged;

		public ButtonBase()
		{
			Toggleable = false;
			ReleaseOnTouchLeft = true;
		}

		public void DoClick()
		{
			OnTouchDown();
			OnTouchUp();
		}

		public virtual void OnPressedChanged()
		{
			PressedChanged.Invoke(this);
		}

		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			if (ReleaseOnTouchLeft && !Toggleable)
			{
				IsPressed = false;
			}
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			if (!Enabled)
			{
				return;
			}

			var wasIsPressed = IsPressed;
			if (!Toggleable)
			{
				IsPressed = false;
			}

			if (wasIsPressed)
			{
				Click.Invoke(this);
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (!Enabled)
			{
				return;
			}

			if (!Toggleable)
			{
				IsPressed = true;
			}
			else
			{
				IsPressed = !IsPressed;
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == Keys.Space)
			{
				if (!Toggleable)
				{
					// Emulate click
					DoClick();
				}
				else
				{
					IsPressed = !IsPressed;
				}
			}
		}

		public override IBrush GetCurrentBackground()
		{
			var result = base.GetCurrentBackground();

			if (Enabled)
			{
				if (IsPressed && PressedBackground != null)
				{
					result = PressedBackground;
				}
				else if (UseHoverRenderable && OverBackground != null)
				{
					result = OverBackground;
				}
			}
			else
			{
				if (DisabledBackground != null)
				{
					result = DisabledBackground;
				}
			}

			return result;
		}

		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyWidgetStyle(style);

			PressedBackground = style.PressedBackground;
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			IsPressed = false;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}