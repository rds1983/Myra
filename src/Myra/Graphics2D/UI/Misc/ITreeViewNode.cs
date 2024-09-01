namespace Myra.Graphics2D.UI
{
	public interface ITreeViewNode
	{
		int ChildNodesCount { get; }

		TreeViewNode AddSubNode(Widget content);
		TreeViewNode GetSubNode(int index);

		void RemoveAllSubNodes();
	}
}
