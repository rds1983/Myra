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
	/// Style class that defines the visual appearance of text box widgets.
	/// </summary>
	public class TextBoxStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the color of the text box's text.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color TextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the text box's text when disabled, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? DisabledTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the text box's text when the widget has focus, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? FocusedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the text box's text when the mouse is over the widget, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? OverTextColor { get; set; }

		/// <summary>
		/// Gets or sets the color of the text box's text when the widget is pressed, or null to use the default.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? PressedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the font used to render the text box's text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase Font { get; set; }

		/// <summary>
		/// Gets or sets the font used to render the hint/placeholder text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase MessageFont { get; set; }

		/// <summary>
		/// Gets or sets the image used to draw the text cursor.
		/// </summary>
		[Category("Appearance")]
		public IImage Cursor { get; set; }

		/// <summary>
		/// Gets or sets the brush used to highlight selected text.
		/// </summary>
		[Category("Appearance")]
		public IBrush Selection { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBoxStyle"/> class.
		/// </summary>
		public TextBoxStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBoxStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source text box style to copy from.</param>
		public TextBoxStyle(TextBoxStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;

			Font = style.Font;
			MessageFont = style.MessageFont;

			Cursor = style.Cursor;
			Selection = style.Selection;
		}

		/// <summary>
		/// Creates a deep copy of this text box style.
		/// </summary>
		/// <returns>A new TextBoxStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TextBoxStyle(this);
		}
	}
}
