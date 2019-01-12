using Myra.Graphics2D.UI;
using Myra.Samples.RogueEditor.Data;

namespace Myra.Samples.RogueEditor.UI
{
	public class Explorer: Pane
	{
		private Module _project;

		public Module Project
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

				Rebuild();
			}
		}

		public Tree Tree
		{
			get { return (Tree) Widget; }
		}

		public TreeNode SelectedNode
		{
			get
			{
				return Tree.SelectedRow;
			}
			set
			{
				Tree.SelectedRow = value;
			}
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
			Widget = new Tree();
		}

		public TreeNode AddItemToTree(ItemWithId item, TreeNode root)
		{
			var node = root.AddSubNode(item.Id);
			item.IdChanged += (sender, args) =>
			{
				node.Text = item.Id;
			};

			node.Tag = item;

			return node;
		}

		private void Rebuild()
		{
			var root = Tree;
			Tree.RemoveAllSubNodes();

			if (_project == null)
			{
				return;
			}

			root.Text = "Project";

			var tileInfosNode = root.AddSubNode("TileInfos");
			foreach (var tileInfo in _project.TileInfos.Values)
			{
				AddItemToTree(tileInfo, tileInfosNode);
			}

			var mapsNode = root.AddSubNode("Maps");
			foreach (var map in _project.Maps.Values)
			{
				AddItemToTree(map, mapsNode);
			}
		}
	}
}
