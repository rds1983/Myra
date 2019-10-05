namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle: ControlStyle
	{
		public IRenderable PressedBackground { get; set; }

		public ButtonStyle()
		{
		}

		public ButtonStyle(ButtonStyle style): base(style)
		{
			PressedBackground = style.PressedBackground;
		}

		public override ControlStyle Clone()
		{
			return new ButtonStyle(this);
		}
	}
}
