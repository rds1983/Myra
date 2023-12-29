using Myra.Graphics2D.UI.Styles;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.ColorPicker
{
	public class ColorPickerDialog : Dialog
	{
		public ColorPickerPanel ColorPickerPanel { get; }

		public Color Color
		{
			get
			{
				return ColorPickerPanel.Color;
			}

			set
			{
				ColorPickerPanel.Color = value;
			}
		}

		public ColorPickerDialog(): base(null)
		{
			ColorPickerPanel = new ColorPickerPanel();

			Title = "Color Picker";
			Content = ColorPickerPanel;

			SetStyle(Stylesheet.DefaultStyleName);
		}

		public override void Close()
		{
			base.Close();

			for (var i = 0; i < ColorPickerPanel.UserColors.Length; ++i)
			{
				var colorDisplay = ColorPickerPanel.GetUserColorImage(i);
				var color = colorDisplay.Color;
				var alpha = (int) (colorDisplay.Opacity * 255);
				ColorPickerPanel.UserColors[i] = new Color(color.R, color.G, color.B, alpha);
			}
		}

		public void ApplyColorPickerDialogStyle(ColorPickerDialogStyle style)
		{
			ApplyWindowStyle(style);

			ColorPickerPanel.ApplyColorPickerDialogStyle(style);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyColorPickerDialogStyle(stylesheet.ColorPickerDialogStyles.SafelyGetStyle(name));
		}
	}
}