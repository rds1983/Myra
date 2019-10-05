#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Graphics;
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class TextBoxStyle: ControlStyle
	{
		public Color TextColor { get; set; }
		public Color? DisabledTextColor { get; set; }
		public Color? FocusedTextColor { get; set; }

		public SpriteFont Font { get; set; }
		public SpriteFont MessageFont { get; set; }

		public IRenderable Cursor { get; set; }
		public IRenderable Selection { get; set; }

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

		public override ControlStyle Clone()
		{
			return new TextBoxStyle(this);
		}
	}
}
