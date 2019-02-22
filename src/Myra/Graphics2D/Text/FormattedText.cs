namespace Myra.Graphics2D.Text
{
	public class FormattedText : FormattedTextBase<TextLine>
	{
		public FormattedText() : base((font, text, size) => new TextLine(font, text, size))
		{
		}
	}
}