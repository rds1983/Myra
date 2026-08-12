namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents an item that can be selected or deselected in a selector widget.
	/// </summary>
	public interface ISelectorItem
	{
		/// <summary>
		/// Gets or sets a value indicating whether this item is selected.
		/// </summary>
		bool IsSelected
		{
			get; set;
		}
	}
}
