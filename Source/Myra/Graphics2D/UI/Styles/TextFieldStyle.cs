using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class TextFieldStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color DisabledTextColor { get; set; }
		public Color FocusedTextColor { get; set; }
		public Color MessageTextColor { get; set; }

		public SpriteFont Font { get; set; }
		public SpriteFont MessageFont { get; set; }

		public TextureRegion Cursor { get; set; }
		public TextureRegion Selection { get; set; }

		public TextFieldStyle()
		{
		}

		public TextFieldStyle(TextFieldStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;
			MessageTextColor = style.MessageTextColor;

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
