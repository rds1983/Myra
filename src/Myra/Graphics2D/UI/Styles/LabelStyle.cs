using FontStashSharp;
using System.ComponentModel;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of label and text widgets.
	/// </summary>
	public class LabelStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the color of the label's text.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color TextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the label's text when the widget is disabled, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? DisabledTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the label's text when the widget has focus, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? FocusedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the label's text when the mouse is over the widget, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? OverTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the label's text when the widget is pressed, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? PressedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the font used to render the label's text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase Font { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LabelStyle"/> class.
		/// </summary>
		public LabelStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LabelStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source label style to copy from.</param>
		public LabelStyle(LabelStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		/// <summary>
		/// Creates a deep copy of this label style.
		/// </summary>
		/// <returns>A new LabelStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new LabelStyle(this);
		}
	}
}
