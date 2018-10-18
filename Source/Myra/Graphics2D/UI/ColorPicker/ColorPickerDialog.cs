using Microsoft.Xna.Framework;
using Myra.Utility;
using System;

namespace Myra.Graphics2D.UI.ColorPicker
{
	public partial class ColorPickerDialog : Dialog
	{
		private const string HexChars = "1234567890ABCDEFabcdef";
		public static readonly Color[] UserColors = new []
		{
			Color.Red,
			Color.Green,
			Color.Blue,
			Color.Cyan,
			Color.White,
			Color.Black,
			Color.HotPink,
			Color.Indigo,
			Color.Orange,
			Color.LightBlue,
			Color.Olive,
			Color.SkyBlue
		};

		private bool _suppressHSV = false;

		public Color Color
		{
			get
			{
				return _imageColor.Color;
			}

			set
			{
				if (value == _imageColor.Color)
				{
					return;
				}

				_imageColor.Color = value;

				OnColorChanged();
			}
		}

		public byte R
		{
			get
			{
				return Color.R;
			}

			set
			{
				Color = new Color(value, Color.G, Color.B, Color.A);
			}
		}

		public byte G
		{
			get
			{
				return Color.G;
			}

			set
			{
				Color = new Color(Color.R, value, Color.B, Color.A);
			}
		}

		public byte B
		{
			get
			{
				return Color.B;
			}

			set
			{
				Color = new Color(Color.R, Color.G, value, Color.A);
			}
		}

		public byte A
		{
			get
			{
				return Color.A;
			}

			set
			{
				Color = new Color(Color.R, Color.G, Color.B, value);
			}
		}

		private int? SelectedUserColorIndex
		{
			get
			{
				if (_gridUserColors.SelectedColumnIndex == null || _gridUserColors.SelectedRowIndex == null)
				{
					return null;
				}
				var index = _gridUserColors.SelectedRowIndex.Value * _gridUserColors.ColumnsProportions.Count + _gridUserColors.SelectedColumnIndex.Value;

				return index;
			}
		}

		public ColorPickerDialog()
		{
			BuildUI();

			// Users colors
			for(var row = 0; row < _gridUserColors.RowsProportions.Count; ++row)
			{
				for (var col = 0; col < _gridUserColors.ColumnsProportions.Count; ++col)
				{
					var image = new Image
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						GridPositionX = col,
						GridPositionY = row,
						TextureRegion = DefaultAssets.WhiteRegion,
						Color = Color.White,
						PaddingLeft = 8,
						PaddingRight = 8,
						PaddingTop = 8,
						PaddingBottom = 8
					};

					_gridUserColors.Widgets.Add(image);
				}
			}

			for (var i = 0; i < UserColors.Length; ++i)
			{
				SetUserColor(i, UserColors[i]);
			}

			_gridUserColors.SelectedIndexChanged += GridUserColorsSelectedIndexChanged;
			_buttonSaveColor.Down += ButtonSaveColorDown;
			UpdateEnabled();

			// Subscriptions
			SubscribeRGB(_spinButtonR, _sliderR, value => R = value);
			SubscribeRGB(_spinButtonG, _sliderG, value => G = value);
			SubscribeRGB(_spinButtonB, _sliderB, value => B = value);
			SubscribeRGB(_spinButtonA, _sliderA, value => A = value);

			SubscribeHSV(_spinButtonH, _sliderH);
			SubscribeHSV(_spinButtonS, _sliderS);
			SubscribeHSV(_spinButtonV, _sliderV);

			_textFieldHex.Tag = false;
			_textFieldHex.TextChangedByUser += TextFieldHexTextChangedByUser;
			_textFieldHex.InputFilter = s =>
			{
				if (s == null || s.Length > 8)
				{
					return null;
				}

				for(var i = 0; i < s.Length; ++i)
				{
					if (HexChars.IndexOf(s[i]) == -1)
					{
						return null;
					}
				}

				return s.ToUpper();
			};

			// Set default value
			_imageColor.TextureRegion = DefaultAssets.WhiteRegion;
			Color = Color.White;

			OnColorChanged();
		}

		private void ButtonSaveColorDown(object sender, EventArgs e)
		{
			var index = SelectedUserColorIndex;
			if (index != null)
			{
				SetUserColor(index.Value, Color);
			}
		}

		private void GridUserColorsSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateEnabled();

			var index = SelectedUserColorIndex;
			if (index != null)
			{
				Color = GetUserColor(index.Value);
			}
		}

		private Color GetUserColor(int index)
		{
			return ((Image)_gridUserColors.Widgets[index]).Color;
		}

		private void SetUserColor(int index, Color color)
		{
			((Image)_gridUserColors.Widgets[index]).Color = color;
		}

		private void UpdateEnabled()
		{
			_buttonSaveColor.Enabled = SelectedUserColorIndex != null;
		}

		private void TextFieldHexTextChangedByUser(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_textFieldHex.Text))
			{
				return;
			}

			var s = "#" + _textFieldHex.Text;

			var color = s.FromName();
			if (color == null)
			{
				return;
			}

			try
			{
				_textFieldHex.Tag = true;
				Color = color.Value;
			}
			finally
			{
				_textFieldHex.Tag = false;
			}
		}

		private void SubscribeRGB(SpinButton spinButton, Slider slider, Action<byte> valueSetter)
		{
			spinButton.Tag = false;

			spinButton.ValueChangedByUser += (s, a) =>
			{
				if (spinButton.Value == null)
				{
					return;
				}

				try
				{
					spinButton.Tag = true;
					valueSetter((byte)spinButton.Value.Value);
				}

				finally
				{
					spinButton.Tag = false;
				}
			};

			slider.Tag = false;

			slider.ValueChangedByUser += (s, a) =>
			{
				try
				{
					slider.Tag = true;
					valueSetter((byte)slider.Value);
				}

				finally
				{
					slider.Tag = false;
				}
			};
		}

		private void SubscribeHSV(SpinButton spinButton, Slider slider)
		{
			spinButton.Tag = false;
			slider.Tag = false;

			spinButton.ValueChangedByUser += (s, a) =>
			{
				if (spinButton.Value == null)
				{
					return;
				}

				try
				{
					spinButton.Tag = true;

					var hsv = new ColorHSV
					{
						H = (int)_sliderH.Value,
						S = (int)_sliderS.Value,
						V = (int)_sliderV.Value
					};
					SetHSV(hsv);

					_suppressHSV = true;
					var rgb = hsv.ToRGB();
					Color = new Color(rgb.R, rgb.G, rgb.B, Color.A);
				}
				finally
				{
					spinButton.Tag = false;
					_suppressHSV = false;
				}
			};

			slider.ValueChangedByUser += (s, a) =>
			{
				try
				{
					slider.Tag = true;

					var hsv = new ColorHSV
					{
						H = (int)_sliderH.Value,
						S = (int)_sliderS.Value,
						V = (int)_sliderV.Value
					};
					SetHSV(hsv);

					_suppressHSV = true;
					var rgb = hsv.ToRGB();
					Color = new Color(rgb.R, rgb.G, rgb.B, Color.A);
				}
				finally
				{
					slider.Tag = false;
					_suppressHSV = false;
				}
			};
		}

		private void OnColorChanged()
		{
			if (!(bool)_spinButtonR.Tag)
			{
				_spinButtonR.Value = R;
			}

			if (!(bool)_sliderR.Tag)
			{
				_sliderR.Value = R;
			}

			if (!(bool)_spinButtonG.Tag)
			{
				_spinButtonG.Value = G;
			}

			if (!(bool)_sliderG.Tag)
			{
				_sliderG.Value = G;
			}

			if (!(bool)_spinButtonB.Tag)
			{
				_spinButtonB.Value = B;
			}

			if (!(bool)_sliderB.Tag)
			{
				_sliderB.Value = B;
			}

			if (!(bool)_spinButtonA.Tag)
			{
				_spinButtonA.Value = A;
			}

			if (!(bool)_sliderA.Tag)
			{
				_sliderA.Value = A;
			}

			if (!(bool)_textFieldHex.Tag)
			{
				_textFieldHex.Text = Color.ToHexString().Substring(1);
			}

			if (!_suppressHSV)
			{
				SetHSV(Color.ToHSV());
			}
		}

		private void SetHSV(ColorHSV hsv)
		{
			if (!(bool)_spinButtonH.Tag)
			{
				_spinButtonH.Value = hsv.H;
			}
			if (!(bool)_sliderH.Tag)
			{
				_sliderH.Value = hsv.H;
			}

			if (!(bool)_spinButtonS.Tag)
			{
				_spinButtonS.Value = hsv.S;
			}
			if (!(bool)_sliderS.Tag)
			{
				_sliderS.Value = hsv.S;
			}

			if (!(bool)_spinButtonV.Tag)
			{
				_spinButtonV.Value = hsv.V;
			}
			if (!(bool)_sliderV.Tag)
			{
				_sliderV.Value = hsv.V;
			}
		}

		public override void Close()
		{
			base.Close();

			for (var i = 0; i < UserColors.Length; ++i)
			{
				UserColors[i] = GetUserColor(i);
			}
		}
	}
}