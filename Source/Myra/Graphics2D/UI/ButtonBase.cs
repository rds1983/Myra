using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isPressed;

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
		[JsonIgnore]
		[EditCategory("Appearance")]
		public virtual IRenderable PressedBackground { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public virtual bool Toggleable { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool IgnoreMouseButton { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
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

				if (value)
				{
					FireDown();
				}
				else
				{
					FireUp();
				}
			}
		}

		public event EventHandler Down;
		public event EventHandler Up;

		public ButtonBase()
		{
			Toggleable = false;
		}

		public void Press()
		{
			OnMouseDown(MouseButtons.Left);
			OnMouseUp(MouseButtons.Left);
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			if (IgnoreMouseButton)
			{
				return;
			}

			if (!Toggleable)
			{
				IsPressed = false;
			}
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (!Enabled || IgnoreMouseButton)
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
					IsPressed = true;
					IsPressed = false;
				}
				else
				{
					IsPressed = !IsPressed;
				}
			}
		}

		protected virtual void FireUp()
		{
			var ev = Up;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		protected virtual void FireDown()
		{
			var ev = Down;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public override IRenderable GetCurrentBackground()
		{
			var isOver = IsMouseOver;
			var result = Background;

			if (Enabled)
			{
				if (IsPressed && PressedBackground != null)
				{
					result = PressedBackground;
				}
				else if (isOver && OverBackground != null)
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
	}
}