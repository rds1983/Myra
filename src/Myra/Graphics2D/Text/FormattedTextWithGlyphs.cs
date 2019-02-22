namespace Myra.Graphics2D.Text
{
	public class FormattedTextWithGlyphs: FormattedTextBase<TextLineWithGlyphs>
	{
		public FormattedTextWithGlyphs() : base((font, text, size) => new TextLineWithGlyphs(font, text, size))
		{
		}

		public GlyphInfo GetGlyphInfoByIndex(int charIndex)
		{
			var strings = Strings;

			foreach (var si in strings)
			{
				if (charIndex >= si.Count)
				{
					charIndex -= si.Count;
				}
				else
				{
					return si.GetGlyphInfoByIndex(charIndex);
				}
			}

			return null;
		}
	}
}
