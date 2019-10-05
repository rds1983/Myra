#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Graphics;
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class LabelStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color? DisabledTextColor { get; set; }
		public Color? OverTextColor { get; set; }
		public Color? PressedTextColor { get; set; }
		public SpriteFont Font { get; set; }

		public LabelStyle()
		{
		}

		public LabelStyle(LabelStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		public override WidgetStyle Clone()
		{
			return new LabelStyle(this);
		}
	}
}
