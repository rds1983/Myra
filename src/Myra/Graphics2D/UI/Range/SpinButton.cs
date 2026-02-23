using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Events;
using Myra.Utility.Types;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif
#if MATH_IFACES
using System.Numerics;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An UI element that edits a numeric data type <paramref name="TNum"/>, with up and down buttons.
	/// </summary>
	/// <typeparam name="TNum">The standard number-based value type to hold.</typeparam>
	/// <exception cref="TypeLoadException">Generic <typeparamref name="TNum"/> cannot be <see cref="byte"/> or <see cref="sbyte"/> as they lack math operators and cannot be used.<para/>OR an unknown type was provided.</exception>
	public class SpinButton<TNum> : Widget 
#if MATH_IFACES
		where TNum : struct, INumber<TNum>, IMinMaxValue<TNum>
#else
		where TNum : struct
#endif
	{
#region Statics
		private const string DefaultStringWhole = "0";
		private const string DefaultStringFloat = "0.0";
		private static readonly bool _numTypeHasSign;
		private static readonly bool _numTypeIsFloating;
		private static readonly string _defaultString;
		private static readonly Func<TNum, NumberFormatInfo, string> _strConverter;
		
		static SpinButton() //Static ctor on generics is invoked once per type-instance
		{
			Type arg = typeof(TNum); //Validate generic arg
			if (arg == typeof(byte) || arg == typeof(sbyte))
			{
				throw new TypeLoadException($"Invalid Generic-Type Argument: '{arg}' does not have full math support. Convert to another type first");
			}

			TypeInfo info = TypeHelper<TNum>.Info;
			_numTypeHasSign = info.IsSignedNumber;
			_numTypeIsFloating = info.IsFractionalNumber;
			_defaultString = _numTypeIsFloating ? DefaultStringFloat : DefaultStringWhole;
			
			// Assign strConverter depending on TNum type. object.ToString() can't accept IFormatProvider...
			switch (info.Code)
			{
				// Handle floating point types
				case TypeCode.Single:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is float value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.Double:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is double value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.Decimal:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is decimal value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				
				// Handle signed whole number types
				case TypeCode.Int16:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is short value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.Int32:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is int value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.Int64:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is long value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				
				// Handle unsigned whole number types
				case TypeCode.UInt16:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is ushort value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.UInt32:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is uint value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;
				case TypeCode.UInt64:
					_strConverter = (TNum num, NumberFormatInfo formatter) =>
					{
						if (num is ulong value)
							return value.ToString(formatter);
						throw new TypeAccessException();
					};
					break;

				default:
					throw new TypeLoadException($"Unknown generic numerical type: {arg}");
			}
		}
#endregion
		private readonly GridLayout _layout = new GridLayout();
		private readonly TextBox _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;
		private readonly NumberFormatInfo _formatter;
		private int _decimalPlaces;
		private bool _fixedDigits;
		private TNum _increment;
		private Range<TNum> _range;

		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get => base.HorizontalAlignment;
			set => base.HorizontalAlignment = value;
		}

		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get => base.VerticalAlignment;
			set => base.VerticalAlignment = value;
		}
		
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Nullable { get; set; }

		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Minimum
		{
			get => _range.Min; 
			set => _range.Min = value;
		}
		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Maximum
		{
			get => _range.Max;
			set => _range.Max = value;
		}
		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Value
		{
			get
			{
				return StringToNumber(_textField.Text, Nullable);
			}

			set
			{
				if (!value.HasValue && !Nullable)
				{
					value = default(TNum);
				}

				if (value.HasValue)
				{
					value = _range.Clamp(value.Value);
				}

				_textField.Text = NumberToString(value, Nullable, _formatter);
				
				/*
				if (FixedNumberSize)
				{
					throw new NotImplementedException();
					string majorString = "";
					int k = 0;
					int k2 = 0;
					if (Maximum.HasValue)
					{
						k = MathHelper<TNum>.Abs(Maximum.Value).ToString().Length;
					}
					if (Minimum.HasValue)
					{
						k2 = MathHelper<TNum>.Abs(Minimum.Value).ToString().Length;
					}
					k = k > k2 ? k : k2;
					for (int i = 0; i < k; i++)
					{
						majorString += ZeroChar;
					}
					if (value.HasValue && MathHelper<TNum>.GreaterThanOrEqual(value.Value, MathHelper<TNum>.Zero))
					{
						majorString = SpaceChar + majorString;
					}

					string minorString = DecimalChar.ToString();
					for (int i = 0; i < _decimalPlaces; i++)
					{
						minorString += ZeroChar;
					}
					//_textField.Text = value.HasValue ? value.Value.ToString(MajorString + MinorString) : string.Empty;
				}
				else
				{
					_textField.Text = value.HasValue ? value.Value.ToString() : string.Empty;
				}*/

				if (_textField.Text != null)
				{
					_textField.CursorPosition = 0;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(1)]
		public TNum Increment
		{
			get => _increment;
			set => _increment = value;
		}
		
		[Category("Behavior")]
		[DefaultValue(1)]
		public TNum Mul_Increment { get; set; }

		[Category("Behavior")]
		[DefaultValue(2)]
		public int DecimalPlaces
		{
			get => _fixedDigits ? _decimalPlaces : 0;
			set
			{
				if (_fixedDigits & _numTypeIsFloating)
					value = Math.Abs(value);
				else
					value = 0;
				_decimalPlaces = value;
				_formatter.NumberDecimalDigits = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool FixedNumberSize
		{
			get => _fixedDigits;
			set
			{
				_fixedDigits = value;
				DecimalPlaces = _decimalPlaces;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public TextBox TextBox => _textField;

		protected internal override bool AcceptsMouseWheel => true;

		/// <summary>
		/// Fires when the value is about to be changed
		/// Set Cancel to true if you want to cancel the change
		/// </summary>
		public event EventHandler<ValueChangingEventArgs<TNum?>> ValueChanging;

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<TNum?>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<TNum?>> ValueChangedByUser;

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

			_formatter = new NumberFormatInfo();
			Value = MathHelper<TNum>.Zero;
			Increment = MathHelper<TNum>.One;
			Mul_Increment = MathHelper<TNum>.One;
		}

		private static TNum? StringToNumber(string str, bool isNullable)
		{
			if (!string.IsNullOrEmpty(str))
			{
				if (TypeHelper<TNum>.TryParse(str, out TNum value))
				{
					return value;
				}
			}
			return isNullable ? default(TNum?) : MathHelper<TNum>.Zero;
		}

		private static string NumberToString(TNum? value, bool isNullable, NumberFormatInfo formatter)
		{
			if(formatter == null)
				formatter = NumberFormatInfo.CurrentInfo;
			
			if (value.HasValue)
			{
				TNum num = value.Value;
				return _strConverter.Invoke(num, formatter);
			}

			if (isNullable)
			{
				return string.Empty;
			}
			return _defaultString; // Default value
		}

		private void _textField_ValueChanging(object sender, ValueChangingEventArgs<string> e)
		{
			string str = e.NewValue;
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			
			if (str == "-")
			{
				// Allow prefix 'minus' only if Minimum lower than zero
				if (Minimum.HasValue && MathHelper<TNum>.GreaterThanOrEqual(Minimum.Value, MathHelper<TNum>.Zero))
				{
					e.Cancel = true;
				}
			}
			else
			{
				TNum? newValue = null;
				if (TypeHelper<TNum>.TryParse(str, out TNum num) && _range.IsInRange(num))
				{
					newValue = num;
				}
				else
				{
					e.Cancel = true;
				}

				if (ValueChanging == null)
					return;
				
				var args = new ValueChangingEventArgs<TNum?>(Value, newValue);
				ValueChanging.Invoke(this, args);
				if (args.Cancel)
				{
					e.Cancel = true;
				}
				else
				{
					e.NewValue = args.NewValue.HasValue ? NumberToString(args.NewValue, Nullable, _formatter) : null;
				}
			}
		}

		private void TextBoxOnTextChanged(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChanged?.Invoke(this, new ValueChangedEventArgs<TNum?>(StringToNumber(eventArgs.OldValue, Nullable), StringToNumber(eventArgs.NewValue, Nullable)));
		}

		private void TextBoxOnTextChangedByUser(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<TNum?>(StringToNumber(eventArgs.OldValue, Nullable), StringToNumber(eventArgs.NewValue, Nullable)));
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

		private void SpinValue(bool spinUpward, bool isMouseWheel)
		{
			TNum newValue, delta;
			if (!TypeHelper<TNum>.TryParse(_textField.Text, out newValue))
			{
				newValue = MathHelper<TNum>.Zero;
			}

			if (isMouseWheel)
				delta = MathHelper<TNum>.Multiply(_increment, Mul_Increment);
			else
				delta = _increment;

			if (spinUpward)
				MathHelper<TNum>.Add(ref newValue, delta);
			else
				MathHelper<TNum>.Subtract(ref newValue, delta);

			if (_range.IsInRange(newValue))
			{
				bool changed = MathHelper<TNum>.UnEqual(Value.GetValueOrDefault(), newValue);
				TNum? oldValue = Value;
				Value = newValue;

				if (changed)
				{
					ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<TNum?>(oldValue, newValue));
				}
			}
		}
		private void UpButtonOnUp(object sender, EventArgs eventArgs) 
			=> SpinValue(true, false);
		private void DownButtonOnUp(object sender, EventArgs eventArgs) 
			=> SpinValue(false, false);

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (delta < 0 && _downButton.Visible && _downButton.Enabled)
			{
				SpinValue(false, true);
			}
			else if(delta > 0 && _upButton.Visible && _upButton.Enabled)
			{
				SpinValue(true, true);
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
				string textValue = _defaultString;
				if (Minimum.HasValue && MathHelper<TNum>.GreaterThan(Minimum.Value, MathHelper<TNum>.Zero))
				{
					textValue = NumberToString(Minimum.Value, Nullable, _formatter);
				}
				else if (Maximum.HasValue && MathHelper<TNum>.LessThan(Maximum.Value, MathHelper<TNum>.Zero))
				{
					textValue = NumberToString(Maximum.Value, Nullable, _formatter);
				}

				_textField.Text = textValue;
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
			
			const char sign = '-';
			bool isNegSign = c == sign;
			bool doPrint = false;
			if (_numTypeHasSign)
			{
				if (isNegSign)
				{
					if (_textField.Text.Length == 0 || (_textField.CursorPosition == 0 && _textField.Text[0] != sign))
						doPrint = true;
				}
				else
					doPrint = true;
			}
			else
			{
				if (!isNegSign)
					doPrint = true;
			}
			
			if(doPrint)
				_textField.OnChar(c);
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var spinButton = (SpinButton<TNum>)w;

			Nullable = spinButton.Nullable;
			Minimum = spinButton.Minimum;
			Maximum = spinButton.Maximum;
			Value = spinButton.Value;
			Increment = spinButton.Increment;
			DecimalPlaces = spinButton.DecimalPlaces;
			FixedNumberSize = spinButton.FixedNumberSize;
			Mul_Increment = spinButton.Mul_Increment;
		}
	}
}