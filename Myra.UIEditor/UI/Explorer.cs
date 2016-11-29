using Myra.Graphics2D.UI;

namespace Myra.UIEditor.UI
{
	public class Explorer: Pane<Tree>
	{
		private Project _project;

		public Project Project
		{
			get
			{
				return _project;
			}

			set
			{
				if (value == _project)
				{
					return;
				}

				_project = value;

				Widget.RemoveAllSubNodes();

				if (_project == null)
				{
					return;
				}

				// Root node
				var projectNode = Widget.AddSubNode("Project");
				var rootNode = AddWidget(projectNode, _project.Root);
				Rebuild(rootNode, _project.Root);
			}
		}

		public Explorer()
		{
			Widget = new Tree
			{
				HasRoot = false
			};
		}

		public TreeNode AddWidget(TreeNode root, Widget widget)
		{
			if (widget == null)
			{
				return null;
			}

			var id = widget.GetType().Name;
			if (!string.IsNullOrEmpty(widget.Id))
			{
				id += " (#" + widget.Id + ")";
			}

			var node = root.AddSubNode(id);
			node.Tag = widget;

			return node;
		}

		private void Rebuild(TreeNode node, Widget widget)
		{
			var asContainer = widget as Container;
			if (asContainer == null)
			{
				return;
			}
			
			foreach (var child in asContainer.Items)
			{
				var subNode = AddWidget(node, child);
				Rebuild(subNode, child);
			}
		}
	}
}
