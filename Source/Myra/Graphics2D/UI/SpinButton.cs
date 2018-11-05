using System;
using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class SpinButton: GridBased
	{
		private readonly TextField _textField;
		private readonly ImageButton _upButton;
		private readonly ImageButton _downButton;

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Nullable { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(null)]
		public float? Maximum { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(null)]
		public float? Minimum { get; set; }

		[EditCategory("Behavior")]
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


				_textField.Text = value.HasValue ? value.Value.ToString() : string.Empty;
			}
		}

		public override bool IsFocused
		{
			get { return base.IsFocused; }
			internal set
			{
				base.IsFocused = value;

				if (!value && string.IsNullOrEmpty(_textField.Text) && !Nullable)
				{
					_textField.Text = "0";
				}
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Integer { get; set; }

		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float?>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<float?>> ValueChangedByUser;

		public SpinButton(SpinButtonStyle style): base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			ColumnsProportions.Add(new Proportion());

			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());

			_textField = new TextField
			{
				GridSpanY = 2,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				InputFilter = InputFilter,
				TextVerticalAlignment = VerticalAlignment.Center
			};

			_textField.TextChanged += TextFieldOnTextChanged;
			_textField.TextChangedByUser += TextFieldOnTextChangedByUser;

			Widgets.Add(_textField);

			_upButton = new ImageButton
			{
				GridPositionX = 1,
				ContentVerticalAlignment = VerticalAlignment.Center,
				ContentHorizontalAlignment = HorizontalAlignment.Center
			};
			_upButton.Up += UpButtonOnUp;

			Widgets.Add(_upButton);

			_downButton = new ImageButton
			{
				GridPositionX = 1,
				GridPositionY = 1,
				ContentVerticalAlignment = VerticalAlignment.Center,
				ContentHorizontalAlignment = HorizontalAlignment.Center
			};
			_downButton.Up += DownButtonOnUp;
			Widgets.Add(_downButton);

			if (style != null)
			{
				ApplySpinButtonStyle(style);
			}

			Value = 0;
		}

		public SpinButton(string style)
			: this(Stylesheet.Current.SpinButtonStyles[style])
		{
		}

		public SpinButton()
			: this(Stylesheet.Current.SpinButtonStyle)
		{
		}

		private static float? StringToFloat(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

			return float.Parse(s);
		}

		private void TextFieldOnTextChanged(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			var ev = ValueChanged;
			if (ev != null)
			{
				ev(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
			}
		}

		private void TextFieldOnTextChangedByUser(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			var ev = ValueChangedByUser;
			if (ev != null)
			{
				ev(this, new ValueChangedEventArgs<float?>(StringToFloat(eventArgs.OldValue), StringToFloat(eventArgs.NewValue)));
			}
		}

		private string InputFilter(string s)
		{
			if (string.IsNullOrEmpty(s))
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

		private void UpButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				value = 0;
			}

			++value;
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

			--value;
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

		public void ApplySpinButtonStyle(SpinButtonStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TextFieldStyle != null)
			{
				_textField.ApplyTextFieldStyle(style.TextFieldStyle);
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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySpinButtonStyle(stylesheet.SpinButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.SpinButtonStyles.Keys.ToArray();
		}
	}
}
