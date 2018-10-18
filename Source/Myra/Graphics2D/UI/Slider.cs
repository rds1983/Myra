using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Slider : SingleItemContainer<Button>
	{
		private float _value;
		private int? _mousePos;

		[HiddenInEditor]
		[JsonIgnore]
		public abstract Orientation Orientation { get; }

		[EditCategory("Behavior")]
		public float Minimum { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(100)]
		public float Maximum { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value > Maximum)
				{
					//could throw error instead?
					value = Maximum;
				}

				if (value < Minimum)
				{
					//could throw error instead?
					value = Minimum;
				}

				if (_value == value)
				{
					return;
				}

				_value = value;

				SyncHintWithValue();

				var ev = ValueChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		private int Hint
		{
			get { return Orientation == Orientation.Horizontal ? Widget.XHint : Widget.YHint; }

			set
			{
				if (Hint == value)
				{
					return;
				}

				if (Orientation == Orientation.Horizontal)
				{
					Widget.XHint = value;
				}
				else
				{
					Widget.YHint = value;
				}
			}
		}

		private int MaxHint
		{
			get
			{
				return Orientation == Orientation.Horizontal
					? Bounds.Width - Widget.Bounds.Width
					: Bounds.Height - Widget.Bounds.Height;
			}
		}

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler ValueChangedByUser;

		protected Slider(SliderStyle sliderStyle)
		{
			Widget = new Button();

			Widget.Down += WidgetOnDown;
			Widget.Up += WidgetOnUp;
			if (sliderStyle != null)
			{
				ApplySliderStyle(sliderStyle);
			}

			Maximum = 100;
		}

		private void WidgetOnUp(object sender, EventArgs eventArgs)
		{
			_mousePos = null;
		}

		private void WidgetOnDown(object sender, EventArgs eventArgs)
		{
			_mousePos = GetMousePos();
		}

		private int GetMousePos()
		{
			return Orientation == Orientation.Horizontal ? Desktop.MousePosition.X : Desktop.MousePosition.Y;
		}

		public void ApplySliderStyle(SliderStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.KnobStyle != null)
			{
				Widget.ApplyButtonStyle(style.KnobStyle);
			}
		}

		private void SyncHintWithValue()
		{
			Hint = (int)(MaxHint * (_value / Maximum));
		}

		public override void Arrange()
		{
			base.Arrange();

			SyncHintWithValue();
		}

		public override void OnDesktopChanging()
		{
			base.OnDesktopChanging();

			if (Desktop != null)
			{
				Desktop.MouseMoved -= DesktopMouseMoved;
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			if (Desktop != null)
			{
				Desktop.MouseMoved += DesktopMouseMoved;
			}
		}

		private void DesktopMouseMoved(object sender, GenericEventArgs<Point> e)
		{
			if (_mousePos == null)
			{
				return;
			}

			var mousePos = GetMousePos();
			var delta = mousePos - _mousePos.Value;

			if (delta == 0)
			{
				return;
			}

			var hint = Hint;
			hint += delta;

			if (hint < 0)
			{
				hint = 0;
			}

			if (hint > MaxHint)
			{
				hint = MaxHint;
			}

			var valueChanged = false;
			// Sync Value with Hint
			if (MaxHint != 0)
			{
				var d = Maximum - Minimum;

				var newValue = Minimum + hint * d / MaxHint;
				if (_value != newValue)
				{
					_value = newValue;
					valueChanged = true;
				}
			}

			Hint = hint;

			if (valueChanged)
			{
				var ev = ValueChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}

				ev = ValueChangedByUser;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}

			_mousePos = mousePos;
		}
	}
}