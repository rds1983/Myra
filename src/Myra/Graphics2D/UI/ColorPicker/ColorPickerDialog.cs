using Myra.Utility;
using System;

#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
using ColorHSV = Myra.Utility.ColorHSV;
#endif

namespace Myra.Graphics2D.UI.ColorPicker
{
	public partial class ColorPickerDialog
	{
		private const int Rows = 2;
		private const int ColorsPerRow = 8;
		private const int WheelHeight = 136;
		private const float DegToRad = (float)Math.PI / 180;
		private const float RadToDeg = 180 / (float)Math.PI;
		private const string InputChars = "1234567890,";
		private const string HexChars = "1234567890ABCDEFabcdef";

		public static readonly Color[] UserColors = new[]
		{
			new Color(255, 255, 255),
			new Color(217, 217, 217),
			new Color(178, 178, 178),
			new Color(140, 140, 140),
			new Color(102, 102, 102),
			new Color(64, 64, 64),
			new Color(32, 32, 32),
			new Color(0, 0, 0),
			new Color(254, 57, 48),
			new Color(255, 149, 3),
			new Color(255, 204, 1),
			new Color(75, 217, 97),
			new Color(91, 198, 250),
			new Color(3, 121, 255),
			new Color(87, 86, 213),
			new Color(207, 86, 191)
		};

		public Color Color
		{
			get
			{
				return _colorDisplay.Color;
			}

			set
			{
				if (value == _colorDisplay.Color)
				{
					return;
				}

				OnColorChanged(value);
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
				if (_userColors.SelectedColumnIndex == null || _userColors.SelectedRowIndex == null)
				{
					return null;
				}
				var index = _userColors.SelectedRowIndex.Value * ColorsPerRow + _userColors.SelectedColumnIndex.Value;

				return index;
			}
		}

		private ColorHSV colorHSV;
		private bool hsPickerActive, vPickerActive;

		public ColorPickerDialog()
		{
			BuildUI();

			// Users colors
			for (int row = 0; row < Rows; ++row)
			{
				for (int col = 0; col < ColorsPerRow; ++col)
				{
					var image = new Image {
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						GridRow = row,
						GridColumn = col,
						Renderable = DefaultAssets.WhiteRegion
					};

					_userColors.Widgets.Add(image);
				}
			}
			for (int i = 0; i < UserColors.Length; ++i)
			{
				SetUserColor(i, UserColors[i]);
			}

			_userColors.SelectionHoverBackground = DefaultAssets.UITextureRegionAtlas["button-over"];
			_userColors.SelectionBackground = DefaultAssets.UITextureRegionAtlas["button-down"];
			_userColors.SelectedIndexChanged += GridUserColorsSelectedIndexChanged;
			_saveColor.Click += ButtonSaveColorDown;
			UpdateEnabled();

			// Subscriptions
			_inputRGB.Tag = false;
			_inputRGB.TextChangedByUser += RgbInputChanged;
			_inputRGB.InputFilter = InputFilter;

			_inputHSV.Tag = false;
			_inputHSV.TextChangedByUser += HsvInputChanged;
			_inputHSV.InputFilter = InputFilter;

			_inputHEX.Tag = false;
			_inputHEX.TextChangedByUser += HexInputChanged;
			_inputHEX.InputFilter = s =>
			{
				if (s == null || s.Length > 6)
				{
					return null;
				}

				for (var i = 0; i < s.Length; ++i)
				{
					if (HexChars.IndexOf(s[i]) == -1)
					{
						return null;
					}
				}

				return s.ToUpper();
			};

			_inputAlpha.Tag = false;
			_inputAlpha.TextChangedByUser += AlphaInputChanged;
			_inputAlpha.InputFilter = InputFilter;

			_sliderAlpha.Tag = false;
			_sliderAlpha.ValueChangedByUser += AlphaSliderChanged;

			_hsPicker.Tag = false;
			_vPicker.Tag = false;

			// Set default value
			_colorDisplay.Renderable = DefaultAssets.WhiteRegion;
			_colorBackground.Renderable = DefaultAssets.UITextureRegionAtlas["color-picker-checkerboard"];

			_colorWheel.Renderable = DefaultAssets.UITextureRegionAtlas["color-picker-wheel"];
			_colorWheel.TouchDown += (s, e) => hsPickerActive = true;

			_gradient.Renderable = DefaultAssets.UITextureRegionAtlas["color-picker-gradient"];
			_gradient.TouchDown += (s, e) => vPickerActive = true;

			OnColorChanged(Color.White);
		}

		private string InputFilter(string s)
		{
			if (s == null)
			{
				return null;
			}

			for (var i = 0; i < s.Length; ++i)
			{
				if (InputChars.IndexOf(s[i]) == -1)
				{
					return null;
				}
			}

			return s;
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			if (!Desktop.DefaultMouseInfoGetter().IsLeftButtonDown)
			{
				hsPickerActive = false;
				vPickerActive = false;
			}
			if (hsPickerActive)
			{
				HsPickerMove(Desktop.MousePosition);
			}
			if (vPickerActive)
			{
				VPickerMove(Desktop.MousePosition);
			}
		}

		private void HsPickerMove(Point p)
		{
			int r = WheelHeight / 2;
			int x = p.X - _colorWheel.Bounds.Location.X - r - _hsPicker.Bounds.Height / 2;
			int y = p.Y - _colorWheel.Bounds.Location.Y - r - _hsPicker.Bounds.Width / 2;
			float angle = (float)Math.Atan2(x, y);
			float rsquared = Math.Min(x * x + y * y, r * r);
			float radius = (float)Math.Sqrt(rsquared);
			_hsPicker.Left = (int)(radius * Math.Sin(angle));
			_hsPicker.Top = (int)(radius * Math.Cos(angle));

			int h = (int)(angle * RadToDeg) + 90;
			if (h < 0)
			{
				h += 360;
			}
			ColorHSV hsv = new ColorHSV()
			{
				H = h,
				S = (int)(radius / r * 100),
				V = colorHSV.V
			};

			_hsPicker.Tag = true;
			OnColorChanged(hsv);
			_hsPicker.Tag = false;
		}

		private void VPickerMove(Point p)
		{
			int x = p.Y - _gradient.Bounds.Location.Y - _hsPicker.Bounds.Height / 2;
			x = Math.Max(0, Math.Min(x, WheelHeight));
			_vPicker.Top = x;

			ColorHSV hsv = new ColorHSV()
			{
				H = colorHSV.H,
				S = colorHSV.S,
				V = 100 * (WheelHeight - x) / WheelHeight
			};

			_vPicker.Tag = true;
			OnColorChanged(hsv);
			_vPicker.Tag = false;
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
			return ((Image)_userColors.Widgets[index]).Color;
		}

		private void SetUserColor(int index, Color color)
		{
			((Image)_userColors.Widgets[index]).Color = color;
		}

		private void UpdateEnabled()
		{
			_saveColor.Enabled = SelectedUserColorIndex != null;
		}

		private void RgbInputChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_inputRGB.Text))
			{
				return;
			}

			string[] st = _inputRGB.Text.Split(',');
			if (st.Length != 3)
			{
				return;
			}
			if (byte.TryParse(st[0], out byte r) && byte.TryParse(st[1], out byte g) && byte.TryParse(st[2], out byte b))
			{
				_inputRGB.Tag = true;
				OnColorChanged(new Color(r, g, b, A));
				_inputRGB.Tag = false;
			}
		}

		private void HsvInputChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_inputHSV.Text))
			{
				return;
			}

			string[] st = _inputHSV.Text.Split(',');
			if (st.Length != 3)
			{
				return;
			}
			if (int.TryParse(st[0], out int h) && byte.TryParse(st[1], out byte s) && byte.TryParse(st[2], out byte v))
			{
				if (h > 360 || s > 100 || v > 100)
				{
					return;
				}
				ColorHSV hsv = new ColorHSV()
				{
					H = h,
					S = s,
					V = v
				};
				_inputHSV.Tag = true;
				OnColorChanged(hsv);
				_inputHSV.Tag = false;
			}
		}

		private void HexInputChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_inputHEX.Text) || _inputHEX.Text.Length < 6)
			{
				return;
			}

			var color = ColorStorage.FromName('#' + _inputHEX.Text);
			if (color != null)
			{
				_inputHEX.Tag = true;
				OnColorChanged(new Color(color.Value, A));
				_inputHEX.Tag = false;
			}
		}

		private void AlphaInputChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_inputAlpha.Text))
			{
				return;
			}

			if (byte.TryParse(_inputAlpha.Text, out byte alpha))
			{
				_inputAlpha.Tag = true;
				OnColorChanged(new Color(Color, alpha));
				_inputAlpha.Tag = false;
			}
		}

		private void AlphaSliderChanged(object sender, ValueChangedEventArgs<float> e)
		{
			OnColorChanged(new Color(Color, (byte)e.NewValue));
		}

		private void OnColorChanged(Color c)
		{
			OnColorChanged(c, c.ToHSV());
		}

		private void OnColorChanged(ColorHSV c)
		{
			OnColorChanged(new Color(c.ToRGB(), A), c);
		}

		private void OnColorChanged(Color rgb, ColorHSV hsv)
		{
			if (!(bool)_inputRGB.Tag)
			{
				_inputRGB.Text = string.Format("{0},{1},{2}", rgb.R, rgb.G, rgb.B);
			}
			if (!(bool)_inputHSV.Tag)
			{
				_inputHSV.Text = string.Format("{0},{1},{2}", hsv.H, hsv.S, hsv.V);
			}
			if (!(bool)_inputHEX.Tag)
			{
				_inputHEX.Text = rgb.ToHexString().Substring(1, 6);
			}
			if (!(bool)_inputAlpha.Tag)
			{
				_inputAlpha.Text = rgb.A.ToString();
			}
			if (!(bool)_sliderAlpha.Tag)
			{
				_sliderAlpha.Value = rgb.A;
			}
			if (!(bool)_hsPicker.Tag)
			{
				_hsPicker.Top = (int)(hsv.S / 200f * WheelHeight * Math.Sin(DegToRad * (-hsv.H + 180)));
				_hsPicker.Left = (int)(hsv.S / 200f * WheelHeight * Math.Cos(DegToRad * (-hsv.H + 180)));
			}
			if (!(bool)_vPicker.Tag)
			{
				_vPicker.Top = (int)(hsv.V / -100f * WheelHeight) + WheelHeight;
			}

			_colorWheel.Color = new Color(hsv.V / 100f, hsv.V / 100f, hsv.V / 100f);
			_colorDisplay.Color = rgb;
			colorHSV = hsv;
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