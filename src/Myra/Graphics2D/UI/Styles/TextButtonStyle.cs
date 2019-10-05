namespace Myra.Graphics2D.UI.Styles
{
	public class TextButtonStyle : ButtonStyle
	{
		public LabelStyle LabelStyle { get; set; }

		public TextButtonStyle()
		{
		}

		public TextButtonStyle(TextButtonStyle style)
			: base(style)
		{
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
		}

		public TextButtonStyle(ButtonStyle buttonStyle, LabelStyle textBlockStyle): base(buttonStyle)
		{
			LabelStyle = textBlockStyle != null ? new LabelStyle(textBlockStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new TextButtonStyle(this);
		}
	}
}