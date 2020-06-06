namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle: WidgetStyle
	{
		public IBrush PressedBackground { get; set; }

		public LabelStyle LabelStyle { get; set; }

		public ButtonStyle()
		{
		}

		public ButtonStyle(ButtonStyle style): base(style)
		{
			PressedBackground = style.PressedBackground;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new ButtonStyle(this);
		}
	}
}
