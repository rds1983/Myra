namespace Myra.Graphics2D.UI
{
	public interface IMenuItem : IItemWithId
	{
		Menu Menu { get; set; }
		Widget Widget { get; set; }
		char? UnderscoreChar { get; }
	}
}
