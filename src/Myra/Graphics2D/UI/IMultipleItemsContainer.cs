using System.Collections.ObjectModel;

namespace Myra.Graphics2D.UI
{
	public interface IMultipleItemsContainer
	{
		ObservableCollection<Control> Widgets { get; }
	}
}
