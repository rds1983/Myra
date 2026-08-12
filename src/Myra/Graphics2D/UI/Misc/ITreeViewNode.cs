namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents a node in a tree view that can have child nodes.
	/// </summary>
	public interface ITreeViewNode
	{
		/// <summary>
		/// Gets the number of child nodes.
		/// </summary>
		int ChildNodesCount { get; }

		/// <summary>
		/// Adds a sub-node with the specified content.
		/// </summary>
		/// <param name="content">The widget to display as the sub-node's content.</param>
		/// <returns>The newly added tree view node.</returns>
		TreeViewNode AddSubNode(Widget content);

		/// <summary>
		/// Gets the sub-node at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the sub-node.</param>
		/// <returns>The tree view node at the specified index.</returns>
		TreeViewNode GetSubNode(int index);

		/// <summary>
		/// Removes all child nodes.
		/// </summary>
		void RemoveAllSubNodes();
	}
}
