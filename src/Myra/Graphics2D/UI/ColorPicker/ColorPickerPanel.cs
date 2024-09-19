using System;
using System.Collections.Generic;
using FontStashSharp.RichText;
using Myra.Events;
using Myra.Graphics2D.UI.Styles;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

using ColorHSV = Myra.Utility.ColorHSV;

namespace Myra.Graphics2D.UI.ColorPicker
{
	public partial class ColorPickerPanel
	{
		private enum ActiveState
		{
			None,
			ColorPickerActive,
			GradientActive
		}

		private const int Rows = 2;
		private const int ColorsPerRow = 8;
		private const float DegToRad = (float)Math.PI / 180;
		private const float RadToDeg = 180 / (float)Math.PI;
		private const string InputChars = "1234567890,";
		private const string HexChars = "1234567890ABCDEFabcdef";
		private readonly List<Image> _userColorBackgrounds = new List<Image>();
		private readonly List<Image> _userColorDisplays = new List<Image>();
		private ActiveState _activeState;

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
				var c = _colorDisplay.Color;
				return new Color((byte)c.R,
					(byte)c.G,
					(byte)c.B,
					(byte)DisplayAlpha);
			}

			set
			{
				if (value == Color)
				{
					return;
				}

				OnColorChanged(value);
			}
		}

		public byte R
		{
			get => Color.R;
			set => Color = new Color(value, Color.G, Color.B, Color.A);
		}

		public byte G
		{
			get => Color.G;
			set => Color = new Color(Color.R, value, Color.B, Color.A);
		}

		public byte B
		{
			get => Color.B;
			set => Color = new Color(Color.R, Color.G, value, Color.A);
		}

		public float A
		{
			get => _colorDisplay.Opacity;
			set => _colorDisplay.Opacity = value;
		}

		private int DisplayAlpha => (int)(A * 255f);

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

		private int WheelHeight => _colorWheel.Bounds.Height - 11;

		private ColorHSV colorHSV;

		public ColorPickerPanel()
		{
			BuildUI();

			// Users colors
			for (int row = 0; row < Rows; ++row)
			{
				for (int col = 0; col < ColorsPerRow; ++col)
				{
					var background = new Image
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
					};
					Grid.SetRow(background, row);
					Grid.SetColumn(background, col);

					var image = new Image
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						Renderable = Stylesheet.Current.WhiteRegion
					};
					Grid.SetRow(image, row);
					Grid.SetColumn(image, col);

					_userColors.Widgets.Add(background);
					_userColors.Widgets.Add(image);

					_userColorBackgrounds.Add(background);

					// Keep track of the displays so we can
					// change their opacity and color.
					_userColorDisplays.Add(image);
				}
			}
			for (int i = 0; i < UserColors.Length; ++i)
			{
				SetUserColor(i, UserColors[i]);
			}

			_userColors.SelectedIndexChanged += GridUserColorsSelectedIndexChanged;
			_saveColor.Click += ButtonSaveColorDown;
			UpdateEnabled();

			// Subscriptions
			_inputRGB.Tag = false;
			_inputRGB.TextChangedByUser += RgbInputChanged;
			_inputRGB.ValueChanging += _inputRGB_ValueChanging;

			_inputHSV.Tag = false;
			_inputHSV.TextChangedByUser += HsvInputChanged;
			_inputHSV.ValueChanging += _inputRGB_ValueChanging;

			_inputHEX.Tag = false;
			_inputHEX.TextChangedByUser += HexInputChanged;
			_inputHEX.ValueChanging += _inputHEX_ValueChanging;

			_inputAlpha.Tag = false;
			_inputAlpha.TextChangedByUser += AlphaInputChanged;
			_inputAlpha.ValueChanging += _inputRGB_ValueChanging;

			_sliderAlpha.Tag = false;
			_sliderAlpha.ValueChangedByUser += AlphaSliderChanged;

			_hsPicker.Tag = false;
			_vPicker.Tag = false;

			// Set default value
			_colorDisplay.Renderable = Stylesheet.Current.WhiteRegion;

			_colorWheel.TouchDown += (s, a) => HsPickerMove(Desktop.TouchPosition.Value);
			_colorWheel.TouchMoved += (s, a) => HsPickerMove(Desktop.TouchPosition.Value);

			_vPicker.ValueChanged += (s, e) => VPickerMove();

			OnColorChanged(Color.White);
		}

		private void _inputHEX_ValueChanging(object sender, ValueChangingEventArgs<string> e)
		{
			var s = e.NewValue;
			if (s == null)
			{
				return;
			}

			if (s.Length > 6)
			{
				e.Cancel = true;
				return;
			}

			for (var i = 0; i < s.Length; ++i)
			{
				if (HexChars.IndexOf(s[i]) == -1)
				{
					e.Cancel = true;
					break;
				}
			}

			if (!e.Cancel)
			{
				e.NewValue = s.ToUpper();
			}
		}

		private void _inputRGB_ValueChanging(object sender, ValueChangingEventArgs<string> e)
		{
			var s = e.NewValue;
			if (s == null)
			{
				return;
			}

			for (var i = 0; i < s.Length; ++i)
			{
				if (InputChars.IndexOf(s[i]) == -1)
				{
					e.Cancel = true;
					break;
				}
			}
		}

		private void HsPickerMove(Point p)
		{
			_activeState = ActiveState.ColorPickerActive;

			p = _colorWheel.ToLocal(p);

			int r = WheelHeight / 2;
			int x = p.X - r - _hsPicker.Bounds.Height / 2;
			int y = p.Y - r - _hsPicker.Bounds.Width / 2;
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

		private void VPickerMove()
		{
			_activeState = ActiveState.GradientActive;

			ColorHSV hsv = new ColorHSV()
			{
				H = colorHSV.H,
				S = colorHSV.S,
				V = 100 - (int)_vPicker.Value
			};

			_vPicker.Tag = true;
			OnColorChanged(hsv);
			_vPicker.Tag = false;
		}

		private void ProcessTouch()
		{
			var position = Desktop.TouchPosition.Value;
			switch (_activeState)
			{
				case ActiveState.None:
					break;
				case ActiveState.ColorPickerActive:
					HsPickerMove(position);
					break;
				case ActiveState.GradientActive:
					VPickerMove();
					break;
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();
			ProcessTouch();
		}

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();
			ProcessTouch();
		}

		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			_activeState = ActiveState.None;
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_activeState = ActiveState.None;
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
				var userColorImage = GetUserColorImage(index.Value);
				Color = userColorImage.Color;
				A = userColorImage.Opacity;

				// Call this so we can set the alpha slider and
				// other visuals to match the new color and alpha.
				OnColorChanged(Color);
			}
		}

		internal Image GetUserColorImage(int index)
		{
			return _userColorDisplays[index];
		}

		private void SetUserColor(int index, Color color)
		{
			_userColorDisplays[index].Color = color;
			_userColorDisplays[index].Opacity = A;
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
				OnColorChanged(new Color(r, g, b));
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
				var c = color.Value;
				OnColorChanged(new Color(c.R, c.G, c.B));
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
				A = alpha / 255f;
				OnColorChanged(Color);
				_inputAlpha.Tag = false;
			}
		}

		private void AlphaSliderChanged(object sender, ValueChangedEventArgs<float> e)
		{
			A = e.NewValue / 255f;
			OnColorChanged(Color);
		}

		private void OnColorChanged(Color c)
		{
			A = c.A / 255f;
			OnColorChanged(c, ColorHSV.FromRGB(c));
		}

		private void OnColorChanged(ColorHSV h)
		{
			var c = h.ToRGB();
			c = new Color(c.R, c.G, c.B, (byte)255);
			OnColorChanged(c, h);
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
				_inputAlpha.Text = DisplayAlpha.ToString();
			}
			if (!(bool)_sliderAlpha.Tag)
			{
				_sliderAlpha.Value = DisplayAlpha;
			}
			if (!(bool)_hsPicker.Tag)
			{
				_hsPicker.Top = (int)(hsv.S / 200f * WheelHeight * Math.Sin(DegToRad * (-hsv.H + 180)));
				_hsPicker.Left = (int)(hsv.S / 200f * WheelHeight * Math.Cos(DegToRad * (-hsv.H + 180)));
			}
			if (!(bool)_vPicker.Tag)
			{
				_vPicker.Value = (int)(100 - hsv.V);
			}

			_colorWheel.Color = new Color((byte)(hsv.V * 255.0f / 100f), (byte)(hsv.V * 255.0f / 100f), (byte)(hsv.V * 255.0f / 100f));
			_colorDisplay.Color = rgb;

			colorHSV = hsv;
		}

		public void ApplyColorPickerDialogStyle(ColorPickerDialogStyle style)
		{
			foreach (var image in _userColorBackgrounds)
			{
				image.Renderable = style.CheckerBoard;
			}

			_colorBackground.Renderable = style.CheckerBoard;

			_userColors.SelectionHoverBackground = style.SelectionHoverBackground;
			_userColors.SelectionBackground = style.SelectionBackground;

			_colorWheel.Renderable = style.Wheel;
			_vPicker.Background = style.Gradient;

			var vsPickerKnob = (Image)_vPicker.ImageButton.Content;
			vsPickerKnob.Renderable = vsPickerKnob.OverRenderable = vsPickerKnob.PressedRenderable = style.VSPickerKnob;
		}
	}
}