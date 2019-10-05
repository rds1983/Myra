namespace Myra.Graphics2D.UI.Styles
{
	public static class StyleExtensions
	{
		public static ImageButtonStyle ToImageButtonStyle(this ButtonStyle style)
		{
			if (style == null)
			{
				return null;
			}

			return new ImageButtonStyle(style);
		}

		public static ImageTextButtonStyle ToImageTextButtonStyle(this ButtonStyle buttonStyle,
			LabelStyle textBlockStyle)
		{
			if (buttonStyle == null)
			{
				return null;
			}

			return new ImageTextButtonStyle(buttonStyle, textBlockStyle);
		}

		public static TextButtonStyle ToTextButtonStyle(this ButtonStyle buttonStyle, 
			LabelStyle textBlockStyle)
		{
			if (buttonStyle == null)
			{
				return null;
			}

			return new TextButtonStyle(buttonStyle, textBlockStyle);
		}
	}
}