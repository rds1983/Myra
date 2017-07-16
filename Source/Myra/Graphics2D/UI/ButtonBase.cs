using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	[IgnoreChildren]
	public class ButtonBase<T> : SingleItemContainer<T> where T : Widget
	{
		private bool _isPressed;

		[EditCategory("Appearance")]
		public virtual HorizontalAlignment ContentHorizontalAlignment
		{
			get { return Widget.HorizontalAlignment; }
			set { Widget.HorizontalAlignment = value; }
		}

		[EditCategory("Appearance")]
		public virtual VerticalAlignment ContentVerticalAlignment
		{
			get { return Widget.VerticalAlignment; }
			set { Widget.VerticalAlignment = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public virtual TextureRegion2D PressedBackground { get; set; }

		[EditCategory("Behavior")]
		public virtual bool Toggleable { get; set; }

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
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			if (!Enabled)
			{
				return;
			}

			base.OnMouseUp(mb);

			if (!Toggleable)
			{
				IsPressed = false;
			}
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			if (!Enabled)
			{
				return;
			}

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

		public override TextureRegion2D GetCurrentBackground()
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