using System.Collections.ObjectModel;

namespace Myra.Graphics2D.UI
{
	public interface IMenuItemsContainer
	{
		ObservableCollection<IMenuItem> Items { get; }
	}
}
