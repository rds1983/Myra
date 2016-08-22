using System;
using Myra.Graphics2D.UI.Styles;
using Myra.Input;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _wasDown;

		public Drawable PressedBackground { get; set; }
		public bool Toggleable { get; set; }

		public bool IsPressed { get; set; }

		public event EventHandler<GenericEventArgs<MouseButtons>> Down;
		public event EventHandler<GenericEventArgs<MouseButtons>> Up;

		public ButtonBase()
		{
			InputAPI.MouseDown += InputOnMouseDown;
			InputAPI.MouseUp += InputOnMouseUp;

			Toggleable = false;
		}

		private void InputOnMouseUp(object sender, GenericEventArgs<MouseButtons> args)
		{
			if (!Toggleable)
			{
				IsPressed = false;
			}

			if (_wasDown)
			{
				FireUp(args);

				_wasDown = false;
			}
		}

		private void InputOnMouseDown(object sender, GenericEventArgs<MouseButtons> args)
		{
			if (IsOver)
			{
				if (!Toggleable)
				{
					var fireDown = !IsPressed;

					IsPressed = true;

					if (fireDown)
					{
						FireDown(args);
					}
				}
				else
				{
					IsPressed = !IsPressed;

					FireDown(args);
				}

				_wasDown = true;
			}
		}

		protected virtual void FireUp(GenericEventArgs<MouseButtons> args)
		{
			var ev = Up;
			if (ev != null)
			{
				ev(this, args);
			}
		}

		protected virtual void FireDown(GenericEventArgs<MouseButtons> args)
		{
			var ev = Down;
			if (ev != null)
			{
				ev(this, args);
			}
		}

		public override Drawable GetCurrentBackground()
		{
			var isOver = IsOver;

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