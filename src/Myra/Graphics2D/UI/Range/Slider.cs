using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class Slider : Widget
	{
		private readonly SingleItemLayout<Button> _layout;

		private float _value;

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum { get; set; }

		[Category("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum { get; set; }

		[Category("Behavior")]
		[DefaultValue(0.0f)]
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

				var oldValue = _value;
				_value = value;

				SyncHintWithValue();

				ValueChanged?.Invoke(this, new ValueChangedEventArgs<float>(oldValue, value));
			}
		}

		internal int Hint
		{
			get
			{
				return Orientation == Orientation.Horizontal ? ImageButton.Left : ImageButton.Top;
			}

			set
			{
				if (Hint == value)
				{
					return;
				}

				if (Orientation == Orientation.Horizontal)
				{
					ImageButton.Left = value;
				}
				else
				{
					ImageButton.Top = value;
				}
			}
		}

		internal int MaxHint
		{
			get
			{
				return Orientation == Orientation.Horizontal
					? Bounds.Width - ImageButton.Bounds.Width
					: Bounds.Height - ImageButton.Bounds.Height;
			}
		}

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				if (Desktop != null)
				{
					Desktop.TouchMoved -= DesktopTouchMoved;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.TouchMoved += DesktopTouchMoved;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public Button ImageButton => _layout.Child;

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float>> ValueChangedByUser;

		protected Slider(string styleName)
		{
			_layout = new SingleItemLayout<Button>(this)
			{
				Child = new Button(null)
				{
					Content = new Image(),
					ReleaseOnTouchLeft = false
				}
			};

			ChildrenLayout = _layout;

			SetStyle(styleName);

			Maximum = 100;
		}

		private int GetHint()
		{
			if (Desktop == null)
			{
				return 0;
			}

			var pos = ToLocal(Desktop.TouchPosition.Value);

			var bounds = ImageButton.ActualBounds;
			return Orientation == Orientation.Horizontal ? pos.X - bounds.Width / 2 : pos.Y - bounds.Height / 2;
		}

		public void ApplySliderStyle(SliderStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.KnobStyle != null)
			{
				ImageButton.ApplyButtonStyle(style.KnobStyle);

				if (style.KnobStyle.ImageStyle != null)
				{
					var image = (Image)ImageButton.Content;
					image.ApplyPressableImageStyle(style.KnobStyle.ImageStyle);
				}
			}
		}

		private void SyncHintWithValue()
		{
			Hint = (int)(MaxHint * ((_value - Minimum) / (Maximum - Minimum)));
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			SyncHintWithValue();
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			UpdateHint();
			ImageButton.IsPressed = true;
		}

		private void UpdateHint()
		{
			var hint = GetHint();
			if (hint < 0)
			{
				hint = 0;
			}

			if (hint > MaxHint)
			{
				hint = MaxHint;
			}

			var oldValue = _value;
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
				ev?.Invoke(this, new ValueChangedEventArgs<float>(oldValue, _value));

				ev = ValueChangedByUser;
				ev?.Invoke(this, new ValueChangedEventArgs<float>(oldValue, _value));
			}
		}

		private void DesktopTouchMoved(object sender, EventArgs args)
		{
			if (!ImageButton.IsPressed)
			{
				return;
			}

			UpdateHint();
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var slider = (Slider)w;

			Minimum = slider.Minimum;
			Maximum = slider.Maximum;
			Value = slider.Value;
		}
	}
}