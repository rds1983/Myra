using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace Myra.Graphics2D.UI.Styles
{
	public class TextBlockStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color DisabledTextColor { get; set; }
		public BitmapFont Font { get; set; }

		public TextBlockStyle()
		{
		}

		public TextBlockStyle(TextBlockStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			Font = style.Font;
		}

		public override WidgetStyle Clone()
		{
			return new TextBlockStyle(this);
		}
	}
}
