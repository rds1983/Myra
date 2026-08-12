namespace Myra.MML
{
	/// <summary>
	/// Represents an object that has a unique identifier.
	/// </summary>
	public interface IItemWithId
	{
		/// <summary>
		/// Gets or sets the unique identifier for this item.
		/// </summary>
		string Id { get; set; }
	}
}
