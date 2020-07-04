using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Utility;

#if !STRIDE
using Microsoft.Xna.Framework.Input;
#else
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isToggled = false;

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
		public bool IsToggled
		{
			get
			{
				return _isToggled;
			}

			set
			{
				if (value == _isToggled)
				{
					return;
				}

				_isToggled = value;

				OnToggledChanged();
			}
		}

        /// <summary>
        /// Indicates whether the current touch gesture started in this widget.
        /// Similar to <see cref="Widget.TouchStayedInside"/> but stays <see langword="true"/> also if the gesture leaves the widget's bounds.
        /// </summary>
        public bool IsPressed { get; set; }

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

			if (!Active && IsToggled && !Toggleable)
			{
				IsToggled = false;
			}
		}

		public event EventHandler ToggledChanged;

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

		public virtual void OnToggledChanged()
		{
			ToggledChanged.Invoke(this);
		}

		public override void OnTouchLeft(HookableEventArgs args)
		{
			base.OnTouchLeft(args);

			if (ReleaseOnTouchLeft && !Toggleable)
			{
				IsToggled = false;
			}
		}

		public override void OnClick()
		{
			base.OnClick();

            if (Toggleable && CanChangeToggleable(!IsToggled))
            {
                IsToggled = !IsToggled;
            }
		}

        public override void OnTouchDown()
        {
            base.OnTouchDown();

            IsPressed = true;
        }

        public override void OnTouchUp()
        {
            base.OnTouchUp();

            IsPressed = false;
        }

        protected virtual bool CanChangeToggleable(bool value)
		{
			return true;
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
					IsToggled = !IsToggled;
				}
			}
		}

		public override IBrush GetCurrentBackground()
		{
			var result = base.GetCurrentBackground();

			if (Enabled)
			{
				if (IsToggled && PressedBackground != null)
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
			IsToggled = false;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles[name]);
		}
	}
}