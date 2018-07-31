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

				_value = value;

				SyncHintWithValue();
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

				var ev = ValueChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
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

		public event EventHandler ValueChanged;

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

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

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

			Hint = hint;

			// Sync Value with Hint
			if (MaxHint != 0)
			{
				var d = Maximum - Minimum;
				_value = Minimum + Hint * d / MaxHint;
			}

			_mousePos = mousePos;
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
	}
}