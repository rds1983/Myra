namespace StbTextEditSharp
{
	public interface ITextEditHandler
	{
		string Text { get; set; }

		int Length { get; }

		TextEditRow LayoutRow(int startIndex);
		float GetWidth(int index);
	}
}