using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.UI.Styles
{
	public class TextBlockStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color DisabledTextColor { get; set; }
		public SpriteFont Font { get; set; }

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
