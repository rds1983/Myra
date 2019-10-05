namespace Myra.Graphics2D.UI
{
	public interface IMenuItem : IItemWithId
	{
		Menu Menu { get; set; }
		Control Widget { get; set; }
		char? UnderscoreChar { get; }
	}
}
