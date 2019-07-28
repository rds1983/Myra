namespace Myra.Graphics2D.UI.Styles
{
	public class TextButtonStyle : ButtonStyle
	{
		public TextBlockStyle LabelStyle { get; set; }

		public TextButtonStyle()
		{
			LabelStyle = new TextBlockStyle();
		}

		public TextButtonStyle(TextButtonStyle style)
			: base(style)
		{
			LabelStyle = new TextBlockStyle(style.LabelStyle);
		}

		public override WidgetStyle Clone()
		{
			return new TextButtonStyle(this);
		}
	}
}