using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class Tree : TreeNode
	{
		private readonly List<TreeNode> _allNodes = new List<TreeNode>();
		private TreeNode _selectedRow;
		private bool _rowInfosDirty = true;

		public Drawable RowSelectionBackgroundWithoutFocus { get; set; }
		public Drawable RowSelectionBackground { get; set; }

		public List<TreeNode> AllNodes
		{
			get { return _allNodes; }
		}

		private TreeNode HoverRow { get; set; }

		public TreeNode SelectedRow
		{
			get { return _selectedRow; }

			set
			{
				if (value == _selectedRow)
				{
					return;
				}

				_selectedRow = value;

				var ev = SelectionChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public override bool CanFocus
		{
			get { return true; }
		}

		public event EventHandler SelectionChanged;

		public Tree(TreeStyle style) : base(style, null)
		{
			if (style != null)
			{
				ApplyTreeStyle(style);
			}
		}

		public Tree() : this(DefaultAssets.UIStylesheet.TreeStyle)
		{
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (mb != MouseButtons.Left)
			{
				return;
			}

			if (HoverRow != null)
			{
				if (!HoverRow.RowVisible)
				{
					return;
				}

				if (HoverRow.Mark.AbsoluteBounds.Contains(Desktop.MousePosition))
				{
					return;
				}

				SelectedRow = HoverRow;
			}
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			HoverRow = null;

			if (!AbsoluteBounds.Contains(position))
			{
				return;
			}

			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible && rowInfo.RowBounds.Contains(position))
				{
					HoverRow = rowInfo;
					return;
				}
			}
		}

		public override void RemoveAllSubNodes()
		{
			base.RemoveAllSubNodes();

			_allNodes.Clear();
			_allNodes.Add(this);

			_selectedRow = null;
			HoverRow = null;
		}

		private bool Iterate(TreeNode node, Func<TreeNode, bool> action)
		{
			if (!action(node))
			{
				return false;
			}

			foreach (var widget in node.ChildNodesGrid.Widgets)
			{
				var subNode = (TreeNode)widget;
				if (!Iterate(subNode, action))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Iterates through all nodes
		/// </summary>
		/// <param name="action">Called for each node, returning false breaks iteration</param>
		public void Iterate(Func<TreeNode, bool> action)
		{
			Iterate(this, action);
		}

		private static void RecursiveUpdateRowVisibility(TreeNode tree)
		{
			

			tree.RowVisible = true;

			if (tree.IsExpanded)
			{
				foreach (var widget in tree.ChildNodesGrid.Widgets)
				{
					var treeNode = (TreeNode) widget;
					RecursiveUpdateRowVisibility(treeNode);
				}
			}
		}

		private void UpdateRowInfos()
		{
			var bounds = AbsoluteBounds;

			foreach (var rowInfo in _allNodes)
			{
				rowInfo.RowVisible = false;
			}

			RecursiveUpdateRowVisibility(this);

			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible)
				{
					rowInfo.RowBounds = new Rectangle(bounds.X, rowInfo.AbsoluteBounds.Y, bounds.Width,
						rowInfo.GetRowHeight(0));
				}
			}
		}

		public override void FireLocationChanged()
		{
			base.FireLocationChanged();

			_rowInfosDirty = true;
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			_rowInfosDirty = true;
		}

		public override void InternalRender(SpriteBatch batch, Rectangle bounds)
		{
			if (_rowInfosDirty)
			{
				UpdateRowInfos();
				_rowInfosDirty = false;
			}


			if (SelectedRow != null && SelectedRow.RowVisible && RowSelectionBackground != null)
			{
				if (!IsFocused && RowSelectionBackgroundWithoutFocus != null)
				{
					RowSelectionBackgroundWithoutFocus.Draw(batch, SelectedRow.RowBounds);
				}
				else
				{
					RowSelectionBackground.Draw(batch, SelectedRow.RowBounds);
				}
			}

			base.InternalRender(batch, bounds);
		}

		public void ApplyTreeStyle(TreeStyle style)
		{
			ApplyTreeNodeStyle(style);

			RowSelectionBackground = style.RowSelectionBackground;
			RowSelectionBackgroundWithoutFocus = style.RowSelectionBackgroundWithoutFocus;
		}

		private static bool FindPath(Stack<TreeNode> path, TreeNode node)
		{
			var top = path.Peek();

			for (var i = 0; i < top.ChildNodesCount; ++i)
			{
				var child = top.GetSubNode(i);

				if (child == node)
				{
					return true;
				}

				path.Push(child);

				if (FindPath(path, node))
				{
					return true;
				}

				path.Pop();
			}

			return false;
		}
		

		/// <summary>
		/// Expands path to the node
		/// </summary>
		/// <param name="node"></param>
		public void ExpandPath(TreeNode node)
		{
			var path = new Stack<TreeNode>();

			path.Push(this);

			if (!FindPath(path, node))
			{
				// Path not found
				return;
			}

			while (path.Count > 0)
			{
				var p = path.Pop();
				p.IsExpanded = true;
			}
		}
	}
}