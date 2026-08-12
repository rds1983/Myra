using System;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Events;
using System.Collections;


#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A spin button widget for entering numeric values with increment/decrement buttons.
	/// </summary>
	public class SpinButton : Widget
	{
		private readonly GridLayout _layout = new GridLayout();
		private readonly TextBox _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;
		private bool _integer = false;
		private int _decimalPlaces = 0;
		private float _increment = 1f;

		/// <summary>
		/// Gets or sets a value indicating whether null is an acceptable value for this spin button. Default is false.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Nullable { get; set; }

		/// <summary>
		/// Gets or sets the maximum value allowed in the spin button, or null for no maximum limit.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Maximum { get; set; }

		/// <summary>
		/// Gets or sets the minimum value allowed in the spin button, or null for no minimum limit.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Minimum { get; set; }

		/// <summary>
		/// Gets or sets the horizontal alignment of the spin button. Default is Left.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the spin button. Default is Top.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the current numeric value of the spin button. Default is 0.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float? Value
		{
			get
			{
				if (string.IsNullOrEmpty(_textField.Text))
				{
					return Nullable ? default(float?) : 0.0f;
				}

				float result;
				if (float.TryParse(_textField.Text, out result))
				{
					return result;
				}

				return null;
			}

			set
			{
				if (FixedNumberSize)
				{
					string MajorString = "";
					int k = 0;
					int k2 = 0;
					if (Maximum.HasValue)
					{
						k = Math.Abs(Maximum.Value).ToString().Count();
					}
					if (Minimum.HasValue)
					{
						k2 = Math.Abs(Minimum.Value).ToString().Count();
					}
					k = k > k2 ? k : k2;
					for (int i = 0; i < k; i++)
					{
						MajorString += "0";
					}
					if (value.HasValue && value.Value >= 0)
					{
						MajorString = " " + MajorString;
					}
					string MinorString = ".";
					for (int i = 0; i < _decimalPlaces; i++)
					{
						MinorString += "0";
					}
					_textField.Text = value.HasValue ? value.Value.ToString(MajorString + MinorString) : string.Empty;
				}
				else
				{
					_textField.Text = value.HasValue ? value.Value.ToString() : string.Empty;
				}

				if (_textField.Text != null)
				{
					_textField.CursorPosition = 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the amount to increment or decrement the value when using the up/down buttons. Default is 1.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(1f)]
		public float Increment
		{
			get
			{
				return _increment;
			}

			set
			{
				if (Integer)
				{
					_increment = (int)value;
				}
				else
				{
					_increment = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of decimal places to display for floating-point values. Default is 0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0)]
		public int DecimalPlaces
		{
			get
			{
				return _decimalPlaces;
			}

			set
			{
				if (Integer)
				{
					_decimalPlaces = 0;
				}
				else
				{
					_decimalPlaces = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the spin button should use fixed-width number formatting.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool FixedNumberSize { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the spin button should only accept integer values.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Integer
		{
			get
			{
				return _integer;
			}

			set
			{
				_integer = value;
				if (Integer)
				{
					_increment = (int)_increment;
					Value = (int)Value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the multiplier applied to the increment when using the mouse wheel. Default is 1.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(1f)]
		public float Mul_Increment { get; set; } = 1f;

		/// <summary>
		/// Gets the text box widget that contains the spin button's text input.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public TextBox TextBox => _textField;

		/// <summary>
		/// Gets a value indicating whether the spin button accepts mouse wheel input.
		/// </summary>
		protected internal override bool AcceptsMouseWheel => true;

		/// <summary>
		/// Occurs before the value is changed. Set Cancel to true to prevent the change.
		/// </summary>
		public event MyraEventHandler<ValueChangingEventArgs<float?>> ValueChanging;

		/// <summary>
		/// Occurs when the value changes, regardless of whether it was changed by user input or programmatically.
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float?>> ValueChanged;

		/// <summary>
		/// Occurs when the value changes due to user interaction (buttons or mouse wheel). Does not fire for programmatic value changes.
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float?>> ValueChangedByUser;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpinButton"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public SpinButton(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			AcceptsKeyboardFocus = true;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			_layout.ColumnsProportions.Add(new Proportion());

			_layout.RowsProportions.Add(new Proportion());
			_layout.RowsProportions.Add(new Proportion());

			_textField = new TextBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				TextVerticalAlignment = VerticalAlignment.Center,
				AcceptsKeyboardFocus = false
			};
			Grid.SetRowSpan(_textField, 2);

			_textField.ValueChanging += _textField_ValueChanging;

			_textField.TextChanged += TextBoxOnTextChanged;
			_textField.TextChangedByUser += TextBoxOnTextChangedByUser;

			Children.Add(_textField);

			_upButton = new Button
			{
				Content = new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};
			Grid.SetColumn(_upButton, 1);
			_upButton.Click += UpButtonOnUp;

			Children.Add(_upButton);

			_downButton = new Button
			{
				Content = new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};
			Grid.SetColumn(_downButton, 1);
			Grid.SetRow(_downButton, 1);

			_downButton.Click += DownButtonOnUp;
			Children.Add(_downButton);

			SetStyle(stylesheet, styleName);

			Value = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpinButton"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public SpinButton(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.SpinButtonStyles;

		private static float? StringToFloat(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

			float f;
			if (!float.TryParse(s, out f))
			{
				return null;
			}

			return f;
		}

		private string NumberToString(float? v)
		{
			if (v == null)
			{
				if (Nullable)
				{
					return string.Empty;
				}

				// Default value
				return "0";
			}

			if (Integer)
			{
				return ((int)v.Value).ToString();
			}

			return v.Value.ToString();
		}

		private void _textField_ValueChanging(object sender, ValueChangingEventArgs<string> e)
		{
			var s = e.NewValue;
			if (string.IsNullOrEmpty(s))
			{
			}
			else if (s == "-")
			{
				// Allow prefix 'minus' only if Minimum lower than zero
				if (Minimum != null && Minimum.Value >= 0)
				{
					e.Cancel = true;
				}
			}
			else
			{
				float? newValue = null;
				if (Integer)
				{
					int i;
					if (!int.TryParse(s, out i))
					{
						e.Cancel = true;
					}
					else
					{
						if ((Minimum != null && i < (int)Minimum) ||
							(Maximum != null && i > (int)Maximum))
						{
							e.Cancel = true;
						}
						else
						{
							newValue = i;
						}
					}
				}
				else
				{
					float f;
					if (!float.TryParse(s, out f))
					{
						e.Cancel = true;
					}
					else
					{
						if ((Minimum != null && f < Minimum) ||
							(Maximum != null && f > Maximum))
						{
							e.Cancel = true;
						}
						else
						{
							newValue = f;
						}
					}
				}

				if (newValue != null)
				{
				}

				// Now SpinButton's 
				if (ValueChanging != null)
				{
					var args = new ValueChangingEventArgs<float?>(Value, newValue);
					ValueChanging(this, args);
					if (args.Cancel)
					{
						e.Cancel = true;
					}
					else
					{
						e.NewValue = args.NewValue.HasValue ? NumberToString(args.NewValue) : null;
					}
				}
			}
		}

		private void TextBoxOnTextChanged(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChanged?.Invoke(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
		}

		private void TextBoxOnTextChangedByUser(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
		}

		private bool InRange(float value)
		{
			if (Minimum.HasValue && value < Minimum.Value)
			{
				return false;
			}

			if (Maximum.HasValue && value > Maximum.Value)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Applies the specified widget style to this spin button.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var spinButtonStyle = (SpinButtonStyle)style;
			if (spinButtonStyle.TextBoxStyle != null)
			{
				_textField.ApplyTextBoxStyle(spinButtonStyle.TextBoxStyle);
			}

			if (spinButtonStyle.UpButtonStyle != null)
			{
				_upButton.ApplyImageButtonStyle(spinButtonStyle.UpButtonStyle);
			}

			if (spinButtonStyle.DownButtonStyle != null)
			{
				_downButton.ApplyImageButtonStyle(spinButtonStyle.DownButtonStyle);
			}
		}

		private void UpButtonOnUp(object sender, MyraEventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}
			value += _increment;
			if (InRange(value))
			{
				var changed = Value != value;
				var oldValue = Value;
				Value = value;

				if (changed)
				{
					var ev = ValueChangedByUser;
					if (ev != null)
					{
						ev(this, new ValueChangedEventArgs<float?>(oldValue, value));
					}
				}
			}
		}
		private void DownButtonOnUp(object sender, MyraEventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}

			value -= _increment;
			if (InRange(value))
			{
				var changed = Value != value;
				var oldValue = Value;
				Value = value;

				if (changed)
				{
					var ev = ValueChangedByUser;
					if (ev != null)
					{
						ev(this, new ValueChangedEventArgs<float?>(oldValue, value));
					}
				}
			}
		}

		/// <summary>
		/// Handles mouse wheel input to adjust the spin button value when the wheel adjustment multiplier is applied.
		/// </summary>
		/// <param name="delta">The mouse wheel delta value (positive for up/forward, negative for down/back).</param>
		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}

			if (delta < 0 && _downButton.Visible && _downButton.Enabled)
			{
				value -= _increment * Mul_Increment;
				if (InRange(value))
				{
					var changed = Value != value;
					var oldValue = Value;
					Value = value;

					if (changed)
					{
						ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<float?>(oldValue, value));
					}
				}
			}
			else if (delta > 0 && _upButton.Visible && _upButton.Enabled)
			{
				value += _increment * Mul_Increment;
				if (InRange(value))
				{
					var changed = Value != value;
					var oldValue = Value;
					Value = value;

					if (changed)
					{
						ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<float?>(oldValue, value));
					}
				}
			}
		}

		/// <summary>
		/// Handles gaining keyboard focus by delegating focus to the text box input.
		/// </summary>
		public override void OnGotKeyboardFocus()
		{
			base.OnGotKeyboardFocus();

			_textField.OnGotKeyboardFocus();
		}

		/// <summary>
		/// Handles losing keyboard focus by delegating to the text box and applying a default value if needed.
		/// </summary>
		public override void OnLostKeyboardFocus()
		{
			base.OnLostKeyboardFocus();

			if (string.IsNullOrEmpty(_textField.Text) && !Nullable)
			{
				var defaultValue = "0";
				if (Minimum != null && Minimum.Value > 0)
				{
					defaultValue = NumberToString(Minimum.Value);
				}
				else if (Maximum != null && Maximum.Value < 0)
				{
					defaultValue = NumberToString(Maximum.Value);
				}

				_textField.Text = defaultValue;
			}

			_textField.OnLostKeyboardFocus();
		}

		/// <summary>
		/// Handles keyboard key down events by delegating to the text box input.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_textField.OnKeyDown(k);
		}

		/// <summary>
		/// Handles character input by delegating to the text box input.
		/// </summary>
		/// <param name="c">The character being entered.</param>
		public override void OnChar(char c)
		{
			base.OnChar(c);

			_textField.OnChar(c);
		}

		/// <summary>
		/// Copies the properties from another spin button widget.
		/// </summary>
		/// <param name="w">The source spin button widget to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var spinButton = (SpinButton)w;

			Nullable = spinButton.Nullable;
			Minimum = spinButton.Minimum;
			Maximum = spinButton.Maximum;
			Value = spinButton.Value;
			Increment = spinButton.Increment;
			DecimalPlaces = spinButton.DecimalPlaces;
			FixedNumberSize = spinButton.FixedNumberSize;
			Integer = spinButton.Integer;
			Mul_Increment = spinButton.Mul_Increment;
		}
	}
}