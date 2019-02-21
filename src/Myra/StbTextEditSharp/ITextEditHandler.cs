namespace StbTextEditSharp
{
	internal interface ITextEditHandler
	{
		string Text { get; set; }

		int Length { get; }
		int NewLineWidth
		{
			get;
		}

		TextEditRow LayoutRow(int startIndex);
		float GetWidth(int index);
	}
}