using System.Collections.Generic;
using Myra.Graphics2D.UI;
using ComboBox = Myra.Graphics2D.UI.ComboBox;
using ListBox = Myra.Graphics2D.UI.ListBox;
using MenuItem = Myra.Graphics2D.UI.MenuItem;
using TreeNode = Myra.Graphics2D.UI.TreeNode;

namespace Myra.UIEditor.UI
{
	public class Explorer : Pane
	{
		private Project _project;

		public Tree Tree
		{
			get { return (Tree)Widget; }
		}

		public Project Project
		{
			get { return _project; }

			set
			{
				if (value == _project)
				{
					return;
				}

				_project = value;

				Tree.RemoveAllSubNodes();

				if (_project == null)
				{
					return;
				}

				// Root node
				var projectNode = Tree.AddSubNode("Project");
				projectNode.Tag = _project;

				if (_project.Root != null)
				{
					var rootNode = AddObject(projectNode, _project.Root);
					Rebuild(rootNode, _project.Root);
				}
			}
		}

		public TreeNode SelectedNode
		{
			get { return Tree.SelectedRow; }
		}

		public object SelectedObject
		{
			get
			{
				var treeNode = SelectedNode;

				return treeNode != null ? treeNode.Tag : null;
			}
		}

		public Explorer()
		{
			Widget = new Tree
			{
				HasRoot = false
			};
		}

		private TreeNode FindNodeByObject(TreeNode node, object item)
		{
			if (node.Tag == item)
			{
				return node;
			}

			for (var i = 0; i < node.ChildNodesCount; ++i)
			{
				var result = FindNodeByObject(node.GetSubNode(i), item);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public TreeNode FindNodeByObject(object item)
		{
			return FindNodeByObject(Tree, item);
		}

		private static void SetNodeText(TreeNode node, object item)
		{
			var id = string.Empty;

			var asMenuItem = item as MenuItem;
			if (asMenuItem != null)
			{
				id = asMenuItem.Id;
			}

			var asWidget = item as Widget;
			if (asWidget != null)
			{
				id = asWidget.Id;
			}

			var text = item.GetType().Name;
			var i = text.IndexOf("`");
			if (i >= 0)
			{
				text = text.Remove(i);
			}

			if (!string.IsNullOrEmpty(id))
			{
				text += " (#" + id + ")";
			}

			node.Text = text;
		}

		public void OnObjectIdChanged(object item)
		{
			// Find node
			var node = FindNodeByObject(item);
			if (node == null)
			{
				return;
			}

			SetNodeText(node, item);
		}

		public TreeNode AddObject(TreeNode root, object item)
		{
			if (item == null)
			{
				return null;
			}

			var node = root.AddSubNode(string.Empty);
			SetNodeText(node, item);
			node.Tag = item;

			return node;
		}

		private void Rebuild(TreeNode node, object item)
		{
			var asMenu = item as IMenuItemsContainer;
			if (asMenu != null)
			{
				foreach (var subItem in asMenu.Items)
				{
					var subNode = AddObject(node, subItem);
					Rebuild(subNode, subItem);
				}
			}
			else
			{
				IEnumerable<Widget> widgets = null;
				if (item is Window)
				{
					widgets = new Widget[] { ((Window)item).Content };
				}
				else if (item is SplitPane)
				{
					widgets = ((SplitPane)item).Widgets;
				}
				else if (item is MultipleItemsContainer && !(item is ListBox) && !(item is ComboBox))
				{
					var container = item as MultipleItemsContainer;
					widgets = container.Children;
				}
				else if (item is ScrollPane)
				{
					widgets = ((ScrollPane)item).Children;
				}

				if (widgets == null)
				{
					return;
				}

				foreach (var child in widgets)
				{
					var subNode = AddObject(node, child);
					Rebuild(subNode, child);
				}
			}
		}
	}
}