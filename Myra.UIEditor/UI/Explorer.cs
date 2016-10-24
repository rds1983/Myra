using Myra.Graphics2D.UI;

namespace Myra.UIEditor.UI
{
	public class Explorer: Pane<Tree>
	{
		private Widget _project;

		public Widget Project
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

				Rebuild(Widget, _project);
			}
		}

		public Explorer()
		{
			Widget = new Tree();
		}

		private void Rebuild(TreeNode node, Widget widget)
		{
			Widget.RemoveAllSubNodes();

			if (_project == null)
			{
				return;
			}

			var id = widget.GetType().Name;

			if (!string.IsNullOrEmpty(widget.Id))
			{
				id += " (#" + widget.Id + ")";
			}

			node.Text = id;
			node.Tag = widget;

			var asContainer = widget as Container;
			if (asContainer == null)
			{
				return;
			}
			
			foreach (var child in asContainer.Items)
			{
				var subNode = node.AddSubNode(string.Empty);
				Rebuild(subNode, child);
			}
		}
	}
}
