using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.UI.Styles
{
	public class TextFieldStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color? DisabledTextColor { get; set; }
		public Color? FocusedTextColor { get; set; }

		public SpriteFont Font { get; set; }
		public SpriteFont MessageFont { get; set; }

		public Drawable Cursor { get; set; }
		public Drawable Selection { get; set; }

		public TextFieldStyle()
		{
		}

		public TextFieldStyle(TextFieldStyle style) : base(style)
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
			return new TextFieldStyle(this);
		}
	}
}
