using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class TextFieldStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color DisabledTextColor { get; set; }
		public Color FocusedTextColor { get; set; }
		public Color MessageTextColor { get; set; }

		public BitmapFont Font { get; set; }
		public BitmapFont MessageFont { get; set; }

		public TextureRegion2D FocusedBackground { get; set; }
		public TextureRegion2D Cursor { get; set; }
		public TextureRegion2D Selection { get; set; }
	}
}
