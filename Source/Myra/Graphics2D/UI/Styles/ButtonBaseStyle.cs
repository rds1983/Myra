using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonBaseStyle: WidgetStyle
	{
		public TextureRegion PressedBackground { get; set; }

		public ButtonBaseStyle()
		{
		}

		public ButtonBaseStyle(ButtonBaseStyle style): base(style)
		{
			PressedBackground = style.PressedBackground;
		}

		public override WidgetStyle Clone()
		{
			return new ButtonBaseStyle(this);
		}
	}
}
