namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle: WidgetStyle
	{
		public ButtonStyle()
		{
		}

		public ButtonStyle(ButtonStyle style): base(style)
		{
		}

		public override WidgetStyle Clone() => new ButtonStyle(this);
	}
}
