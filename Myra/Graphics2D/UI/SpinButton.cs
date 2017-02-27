using System;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class SpinButton: GridBased
	{
		private readonly TextField _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;

		public bool Integer { get; set; }

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
		}

		private bool InputFilter(string s)
		{
			if (Integer)
			{
				int i;
				return int.TryParse(s, out i);
			}

			float f;
			return float.TryParse(s, out f);
		}

		private void UpButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				return;
			}

			++value;
			_textField.Text = value.ToString();
		}
		
		private void DownButtonOnUp(object sender, EventArgs eventArgs)
		{
			float value;
			if (!float.TryParse(_textField.Text, out value))
			{
				return;
			}

			--value;
			_textField.Text = value.ToString();
		}

		public SpinButton() : this(DefaultAssets.UIStylesheet.SpinButtonStyle)
		{
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
