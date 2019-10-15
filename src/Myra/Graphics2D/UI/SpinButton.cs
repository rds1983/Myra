using System;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class SpinButton : SingleItemContainer<Grid>
	{
		private readonly TextBox _textField;
		private readonly ImageButton _upButton;
		private readonly ImageButton _downButton;
		private bool _integer = false;
		private int _decimalPlaces = 0;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Nullable
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Maximum
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(null)]
		public float? Minimum
		{
			get; set;
		}
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

		private float _Increment = 1f;
		[Category("Behavior")]
		[DefaultValue(1f)]
		public float Increment
		{
			get
			{
				return _Increment;
			}

			set
			{
				if (Integer)
				{
					_Increment = (int)value;
				}
				else
				{
					_Increment = value;
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
		public bool FixedNumberSize
		{
			get; set;
		}

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
					_Increment = (int)_Increment;
					Value = (int)Value;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(1f)]
		public float Mul_Increment { get; set; } = 1f;

		internal protected override bool AcceptsKeyboardFocus
		{
			get { return true; }
		}

		internal protected override bool AcceptsMouseWheelFocus
		{
			get { return true; }
		}

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float?>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float?>> ValueChangedByUser;

		public SpinButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Grid();

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			InternalChild.ColumnsProportions.Add(new Proportion());

			InternalChild.RowsProportions.Add(new Proportion());
			InternalChild.RowsProportions.Add(new Proportion());

			_textField = new TextBox
			{
				GridRowSpan = 2,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				InputFilter = InputFilter,
				TextVerticalAlignment = VerticalAlignment.Center
			};

			_textField.TextChanged += TextBoxOnTextChanged;
			_textField.TextChangedByUser += TextBoxOnTextChangedByUser;

			InternalChild.Widgets.Add(_textField);

			_upButton = new ImageButton
			{
				GridColumn = 1,
				ContentVerticalAlignment = VerticalAlignment.Center,
				ContentHorizontalAlignment = HorizontalAlignment.Center
			};
			_upButton.Click += UpButtonOnUp;

			InternalChild.Widgets.Add(_upButton);

			_downButton = new ImageButton
			{
				GridColumn = 1,
				GridRow = 1,
				ContentVerticalAlignment = VerticalAlignment.Center,
				ContentHorizontalAlignment = HorizontalAlignment.Center
			};
			_downButton.Click += DownButtonOnUp;
			InternalChild.Widgets.Add(_downButton);

			SetStyle(styleName);

			Value = 0;
		}

		private static float? StringToFloat(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

			return float.Parse(s);
		}

		private void TextBoxOnTextChanged(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			var ev = ValueChanged;
			if (ev != null)
			{
				ev(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
			}
		}

		private void TextBoxOnTextChangedByUser(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			var ev = ValueChangedByUser;
			if (ev != null)
			{
				ev(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
			}
		}

		private string InputFilter(string s)
		{
			if (string.IsNullOrEmpty(s) || s == "-")
			{
				return s;
			}

			if (Integer)
			{
				int i;
				if (!int.TryParse(s, out i))
				{
					return null;
				}

				return s;
			}

			float f;

			if (!float.TryParse(s, out f))
			{
				return null;
			}

			return s;
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
			ApplySpinButtonStyle(stylesheet.SpinButtonStyles[name]);
		}

		private void UpButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}
			value += _Increment;
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
		private void DownButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}

			value -= _Increment;
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

			if (delta < 0)
			{
				value -= _Increment * Mul_Increment;
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
			else if (delta > 0)
			{
				value += _Increment * Mul_Increment;
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
		}

		public override void OnLostKeyboardFocus()
		{
			base.OnLostKeyboardFocus();

			if (string.IsNullOrEmpty(_textField.Text) && !Nullable)
			{
				_textField.Text = "0";
			}
		}
	}
}