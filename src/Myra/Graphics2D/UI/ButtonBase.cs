using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using System.Linq;
using Myra.Utility;

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

		internal bool ReleaseOnTouchLeft;

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}
			set
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

			if (!Toggleable)
			{
				IsPressed = false;
			}

			Click.Invoke(this);
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
				var value = !IsPressed;
				if (CanChangeToggleable(value))
				{
					IsPressed = value;
				}
			}
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
				else if (Active && UseHoverRenderable && OverBackground != null)
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