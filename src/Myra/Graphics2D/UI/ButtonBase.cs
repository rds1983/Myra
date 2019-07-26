using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isPressed = false;

		[EditCategory("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public virtual HorizontalAlignment ContentHorizontalAlignment
		{
			get { return InternalChild.HorizontalAlignment; }
			set { InternalChild.HorizontalAlignment = value; }
		}

		[EditCategory("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public virtual VerticalAlignment ContentVerticalAlignment
		{
			get { return InternalChild.VerticalAlignment; }
			set { InternalChild.VerticalAlignment = value; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public virtual IRenderable PressedBackground { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public virtual bool Toggleable { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool IgnoreMouseButton { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public bool IsPressed
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

		internal bool ReleaseOnMouseLeft
		{
			get; set;
		}

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}
			set
			{
				// If we're not releasing the button on mouse left,
				// we have to do it on touch up
				if (!ReleaseOnMouseLeft && Desktop != null)
				{
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (!ReleaseOnMouseLeft && Desktop != null)
				{
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		protected virtual bool CanChangePressedOnTouchUp
		{
			get
			{
				return true;
			}
		}

		public event EventHandler Click;
		public event EventHandler PressedChanged;

		public ButtonBase()
		{
			Toggleable = false;
			ReleaseOnMouseLeft = true;
		}

		public void DoClick()
		{
			OnTouchDown();
			OnTouchUp();
		}

		public virtual void OnPressedChanged()
		{
			var ev = PressedChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			if (ReleaseOnMouseLeft && !Toggleable)
			{
				IsPressed = false;
			}
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			if (!Enabled || IgnoreMouseButton)
			{
				return;
			}

			if (CanChangePressedOnTouchUp)
			{
				if (!Toggleable)
				{
					IsPressed = false;
				}
				else
				{
					IsPressed = !IsPressed;
				}
			}

			var ev = Click;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (!Enabled || IgnoreMouseButton)
			{
				return;
			}

			if (!Toggleable)
			{
				IsPressed = true;
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

		public override IRenderable GetCurrentBackground()
		{
			var result = Background;

			if (Enabled)
			{
				if (IsPressed && PressedBackground != null)
				{
					result = PressedBackground;
				}
				else if (Active && IsMouseOver && OverBackground != null)
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

		public void ApplyButtonBaseStyle(ButtonBaseStyle style)
		{
			ApplyWidgetStyle(style);

			PressedBackground = style.PressedBackground;
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			IsPressed = false;
		}
	}
}