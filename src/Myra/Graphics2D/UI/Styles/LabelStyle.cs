using FontStashSharp;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class LabelStyle: WidgetStyle
	{
		public Color TextColor { get; set; }
		public Color? DisabledTextColor { get; set; }
		public Color? OverTextColor { get; set; }
		public Color? PressedTextColor { get; set; }
		public SpriteFontBase Font { get; set; }

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
