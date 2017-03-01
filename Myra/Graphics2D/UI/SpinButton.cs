using System;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class SpinButton: GridBased
	{
		private readonly TextField _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;

		public bool Nullable { get; set; }

		public float? Maximum { get; set; }

		public float? Minimum { get; set; }

		public float? Value
		{
			get
			{
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

		public bool Integer { get; set; }

		public event EventHandler ValueChanged;

		public SpinButton(SpinButtonStyle style)
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
				VerticalAlignment = VerticalAlignment.Center,
				InputFilter = InputFilter
			};

			_textField.TextChanged += TextFieldOnTextChanged;

			Widgets.Add(_textField);

			_upButton = new Button
			{
				GridPositionX = 1,
				ContentVerticalAlignment = VerticalAlignment.Center,
				ContentHorizontalAlignment = HorizontalAlignment.Center
			};
			_upButton.Up += UpButtonOnUp;

			Widgets.Add(_upButton);

			_downButton = new Button
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

		public SpinButton()
			: this(DefaultAssets.UIStylesheet.SpinButtonStyle)
		{
		}

		private void TextFieldOnTextChanged(object sender, EventArgs eventArgs)
		{
			var ev = ValueChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private bool InputFilter(string s)
		{
			if (Nullable && string.IsNullOrEmpty(s))
			{
				return true;
			}

			if (Integer)
			{
				int i;
				return int.TryParse(s, out i);
			}

			float f;
			return float.TryParse(s, out f);
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
				return;
			}

			++value;
			if (InRange(value))
			{
				Value = value;
			}
		}
		
		private void DownButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				return;
			}

			--value;
			if (InRange(value))
			{
				Value = value;
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
				_upButton.ApplyButtonStyle(style.UpButtonStyle);
			}

			if (style.DownButtonStyle != null)
			{
				_downButton.ApplyButtonStyle(style.DownButtonStyle);
			}
		}
	}
}
