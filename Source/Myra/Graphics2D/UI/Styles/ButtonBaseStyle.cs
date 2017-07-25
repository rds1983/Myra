using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonBaseStyle: WidgetStyle
	{
		public TextureRegion2D PressedBackground { get; set; }

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
