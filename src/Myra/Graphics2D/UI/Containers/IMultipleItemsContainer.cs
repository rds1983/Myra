using System.Collections.ObjectModel;

namespace Myra.Graphics2D.UI
{
	public interface IMultipleItemsContainer
	{
		ObservableCollection<Widget> Widgets { get; }
		T AddChild<T>(T widget) where T : Widget;
	}
}
