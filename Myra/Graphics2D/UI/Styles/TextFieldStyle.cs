using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;

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

		public Drawable FocusedBackground { get; set; }
		public Drawable Cursor { get; set; }
		public Drawable Selection { get; set; }
	}
}
