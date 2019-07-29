namespace Myra.Graphics2D.UI.Styles
{
	public class TextButtonStyle : ButtonStyle
	{
		public TextBlockStyle LabelStyle { get; set; }

		public TextButtonStyle()
		{
		}

		public TextButtonStyle(TextButtonStyle style)
			: base(style)
		{
			LabelStyle = style.LabelStyle != null ? new TextBlockStyle(style.LabelStyle) : null;
		}

		public TextButtonStyle(ButtonStyle buttonStyle, TextBlockStyle textBlockStyle): base(buttonStyle)
		{
			LabelStyle = textBlockStyle != null ? new TextBlockStyle(textBlockStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new TextButtonStyle(this);
		}
	}
}