using FontStashSharp;

#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Graphics;
using Stride.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class TextBoxStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color? DisabledTextColor { get; set; }
		public Color? FocusedTextColor { get; set; }

		public SpriteFontBase Font { get; set; }
		public SpriteFontBase MessageFont { get; set; }

		public IImage Cursor { get; set; }
		public IBrush Selection { get; set; }

		public TextBoxStyle()
		{
		}

		public TextBoxStyle(TextBoxStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;

			Font = style.Font;
			MessageFont = style.MessageFont;

			Cursor = style.Cursor;
			Selection = style.Selection;
		}

		public override WidgetStyle Clone()
		{
			return new TextBoxStyle(this);
		}
	}
}
