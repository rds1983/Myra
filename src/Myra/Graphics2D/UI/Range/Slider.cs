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
	/// <summary>
	/// An abstract base class for slider widgets that allow users to select a value within a range.
	/// </summary>
	public abstract class Slider : Widget
	{
		private readonly SingleItemLayout<Button> _layout;

		private float _value, _wheelStep = 1.0f;
		private bool _wheelAdjustment;

		/// <summary>
		/// Gets the orientation of the slider (horizontal or vertical).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Gets or sets the minimum value of the slider. Default is 0.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum { get; set; }

		/// <summary>
		/// Gets or sets the maximum value of the slider. Default is 100.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum { get; set; }

		/// <summary>
		/// Gets or sets the current value of the slider between Minimum and Maximum. Default is 0.0.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether the slider can be adjusted with the mouse wheel. Default is false.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool WheelAdjustment
		{
			get
			{
				return _wheelAdjustment;
			}
			set
			{
				_wheelAdjustment = value;
			}
		}

		/// <summary>
		/// Gets or sets the amount to adjust the value when using the mouse wheel. Default is 1.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(1.0f)]
		public float WheelStep
		{
			get
			{
				return _wheelStep;
			}
			set
			{
				_wheelStep = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the slider accepts mouse wheel input.
		/// </summary>
		protected internal override bool AcceptsMouseWheel
		{
			get
			{
				return _wheelAdjustment;
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
					? Bounds.Width - ImageButton.Bounds.Width - Margin.Width
					: Bounds.Height - ImageButton.Bounds.Height - Margin.Height;
			}
		}

		/// <summary>
		/// Gets or sets the desktop this slider is part of, managing touch event subscriptions.
		/// </summary>
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

		/// <summary>
		/// Gets the button widget that acts as the slider's draggable knob.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Button ImageButton => _layout.Child;

		/// <summary>
		/// Occurs when the value of the slider changes, regardless of whether it was changed by user input or programmatically.
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float>> ValueChanged;

		/// <summary>
		/// Occurs when the value of the slider changes due to user interaction (dragging or mouse wheel). Does not fire for programmatic value changes.
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float>> ValueChangedByUser;

		/// <summary>
		/// Initializes a new instance of the <see cref="Slider"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		protected Slider(Stylesheet stylesheet, string styleName)
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

			SetStyle(stylesheet, styleName);

			Maximum = 100;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Slider"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		protected Slider(string styleName) : this(Stylesheet.Current, styleName)
		{
		}

		private int GetHint()
		{
			if (Desktop == null)
			{
				return 0;
			}

			var pos = ToLocal(Desktop.TouchPosition.Value);

			var bounds = ImageButton.ActualBounds;
			return Orientation == Orientation.Horizontal ? pos.X - bounds.Width / 2 - Margin.Left : pos.Y - bounds.Height / 2 - Margin.Top;
		}

		/// <summary>
		/// Applies the specified widget style to this slider.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var sliderStyle = (SliderStyle)style;
			if (sliderStyle.KnobStyle != null)
			{
				ImageButton.ApplyButtonStyle(sliderStyle.KnobStyle);

				if (sliderStyle.KnobStyle.ImageStyle != null)
				{
					var image = (Image)ImageButton.Content;
					image.ApplyImageStyle(sliderStyle.KnobStyle.ImageStyle);
				}
			}
		}

		private void SyncHintWithValue()
		{
			Hint = (int)(MaxHint * ((_value - Minimum) / (Maximum - Minimum)));
		}

		/// <summary>
		/// Arranges the slider and synchronizes the hint (knob position) with the current value.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();

			SyncHintWithValue();
		}

		/// <summary>
		/// Handles touch down events and activates the slider for dragging.
		/// </summary>
		public override void OnTouchDown()
		{
			base.OnTouchDown();

			UpdateHint();
			ImageButton.IsPressed = true;
		}

		/// <summary>
		/// Handles mouse wheel input to adjust the slider value if wheel adjustment is enabled.
		/// </summary>
		/// <param name="delta">The mouse wheel delta value (positive for up/forward, negative for down/back).</param>
		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (_wheelAdjustment)
			{
				var prevValue = _value;

				if (delta < 0)
				{
					Value -= WheelStep;
				}
				else
				{
					Value += WheelStep;
				}

				if (Value != prevValue)
				{
					var ev = ValueChanged;
					ev?.Invoke(this, new ValueChangedEventArgs<float>(prevValue, _value));

					ev = ValueChangedByUser;
					ev?.Invoke(this, new ValueChangedEventArgs<float>(prevValue, _value));
				}
			}
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

		private void DesktopTouchMoved(object sender, MyraEventArgs args)
		{
			if (!ImageButton.IsPressed)
			{
				return;
			}

			UpdateHint();
		}

		/// <summary>
		/// Copies the properties from another slider widget.
		/// </summary>
		/// <param name="w">The source slider widget to copy from.</param>
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
