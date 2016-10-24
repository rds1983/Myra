using System;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isPressed;

		public HorizontalAlignment ContentHorizontalAlignment
		{
			get { return Widget.HorizontalAlignment; }
			set { Widget.HorizontalAlignment = value; }
		}

		public VerticalAlignment ContentVerticalAlignment
		{
			get { return Widget.VerticalAlignment; }
			set { Widget.VerticalAlignment = value; }
		}

		public Drawable PressedBackground { get; set; }
		public bool Toggleable { get; set; }

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

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			if (!Toggleable)
			{
				IsPressed = false;
			}
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (!IsMouseOver) return;

			if (!Toggleable)
			{
				IsPressed = true;
			}
			else
			{
				IsPressed = !IsPressed;
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

		public override Drawable GetCurrentBackground()
		{
			var isOver = IsMouseOver;

			var result = Background;
			if (!Enabled && DisabledBackground != null)
			{
				result = DisabledBackground;
			}
			else if (IsPressed && PressedBackground != null)
			{
				result = PressedBackground;
			}
			else if (isOver && OverBackground != null)
			{
				result = OverBackground;
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