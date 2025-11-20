using System;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public class SpinButton : Widget
	{
		private readonly GridLayout _layout = new GridLayout();
		private readonly TextBox _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;
		private bool _integer = false;
		private int _decimalPlaces = 0;
		private float _increment = 1f;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Nullable { get; set; }

		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Maximum { get; set; }

		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Minimum { get; set; }

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
				if (value == null && !Nullable)
				{
					throw new Exception("value can't be null when Nullable is false");
				}

				if (value.HasValue && Minimum.HasValue && value.Value < Minimum.Value)
				{
					throw new Exception("Value can't be lower than Minimum");
				}

				if (value.HasValue && Maximum.HasValue && value.Value > Maximum.Value)
				{
					throw new Exception("Value can't be higher than Maximum");
				}

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

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool FixedNumberSize { get; set; }

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

		[Category("Behavior")]
		[DefaultValue(1f)]
		public float Mul_Increment { get; set; } = 1f;

		[XmlIgnore]
		[Browsable(false)]
		public TextBox TextBox => _textField;

		protected internal override bool AcceptsMouseWheel => true;

		/// <summary>
		/// Fires when the value is about to be changed
		/// Set Cancel to true if you want to cancel the change
		/// </summary>
		public event MyraEventHandler<ValueChangingEventArgs<float?>> ValueChanging;

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float?>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<float?>> ValueChangedByUser;

		public SpinButton(string styleName = Stylesheet.DefaultStyleName)
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

			SetStyle(styleName);

			Value = 0;
		}

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

		public void ApplySpinButtonStyle(SpinButtonStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TextBoxStyle != null)
			{
				_textField.ApplyTextBoxStyle(style.TextBoxStyle);
			}

			if (style.UpButtonStyle != null)
			{
				_upButton.ApplyImageButtonStyle(style.UpButtonStyle);
			}

			if (style.DownButtonStyle != null)
			{
				_downButton.ApplyImageButtonStyle(style.DownButtonStyle);
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySpinButtonStyle(stylesheet.SpinButtonStyles.SafelyGetStyle(name));
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

		public override void OnGotKeyboardFocus()
		{
			base.OnGotKeyboardFocus();

			_textField.OnGotKeyboardFocus();
		}

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

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_textField.OnKeyDown(k);
		}

		public override void OnChar(char c)
		{
			base.OnChar(c);

			_textField.OnChar(c);
		}

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