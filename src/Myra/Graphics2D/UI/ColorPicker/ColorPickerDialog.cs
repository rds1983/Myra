using Myra.Graphics2D.UI.Styles;
using System.Collections;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.ColorPicker
{
	/// <summary>
	/// A dialog window for selecting colors with a color picker panel.
	/// </summary>
	public class ColorPickerDialog : Dialog
	{
		/// <summary>
		/// Gets the color picker panel contained in this dialog.
		/// </summary>
		public ColorPickerPanel ColorPickerPanel { get; }

		/// <summary>
		/// Gets or sets the selected color.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorPickerDialog"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public ColorPickerDialog(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, null)
		{
			ColorPickerPanel = new ColorPickerPanel();

			Title = "Color Picker";
			Content = ColorPickerPanel;

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorPickerDialog"/> class.
		/// </summary>
		public ColorPickerDialog() : this(Stylesheet.Current, Stylesheet.DefaultStyleName)
		{
		}

		/// <summary>
		/// Closes the color picker dialog and saves user colors.
		/// </summary>
		public override void Close()
		{
			base.Close();

			for (var i = 0; i < ColorPickerPanel.UserColors.Length; ++i)
			{
				var colorDisplay = ColorPickerPanel.GetUserColorImage(i);
				var color = colorDisplay.Color;
				var alpha = (int)(colorDisplay.Opacity * 255);
				ColorPickerPanel.UserColors[i] = new Color(color.R, color.G, color.B, alpha);
			}
		}


		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.ColorPickerDialogStyles;

		/// <summary>
		/// Applies the specified widget style to this color picker dialog.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var colorPickerDialogStyle = (ColorPickerDialogStyle)style;
			ColorPickerPanel.ApplyColorPickerDialogStyle(colorPickerDialogStyle);
		}
	}
}