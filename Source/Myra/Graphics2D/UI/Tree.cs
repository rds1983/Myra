using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class Tree : TreeNode
	{
		private readonly List<TreeNode> _allNodes = new List<TreeNode>();
		private TreeNode _selectedRow;
		private bool _rowInfosDirty = true;
		private bool _hasRoot = true;

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

		[DefaultValue(true)]
		public override bool CanFocus
		{
			get { return base.CanFocus; }
			set { base.CanFocus = value; }
		}

		[DefaultValue(true)]
		public bool HasRoot
		{
			get
			{
				return _hasRoot;
			}

			set
			{
				if (value == _hasRoot)
				{
					return;
				}

				_hasRoot = value;
				UpdateHasRoot();
			}
		}

		public event EventHandler SelectionChanged;

		public Tree(TreeStyle style):base(style, null)
		{
			CanFocus = true;
			if (style != null)
			{
				ApplyTreeStyle(style);
			}
		}

		public Tree(string style)
			: this(Stylesheet.Current.TreeStyles[style])
		{
		}

		public Tree() : this(Stylesheet.Current.TreeStyle)
		{
		}

		private void UpdateHasRoot()
		{
			if (_hasRoot)
			{
				Mark.Visible = true;
				Label.Visible = true;
				ChildNodesGrid.Visible = Mark.IsPressed;
			}
			else
			{
				Mark.Visible = false;
				Label.Visible = false;
				Mark.IsPressed = true;
				ChildNodesGrid.Visible = true;
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (SelectedRow == null)
			{
				return;
			}

			int index = 0;
			IList<Widget> parentWidgets = null;
			if (SelectedRow.ParentNode != null)
			{
				parentWidgets = SelectedRow.ParentNode.ChildNodesGrid.Widgets;
				index = parentWidgets.IndexOf(SelectedRow);
				if (index == -1)
				{
					return;
				}
			}

			switch (k)
			{
				case Keys.Enter:
					SelectedRow.IsExpanded = !SelectedRow.IsExpanded;
					break;
				case Keys.Up:
				{
					if (parentWidgets != null)
					{
						if (index == 0)
						{
							SelectedRow = SelectedRow.ParentNode;
						}
						else if (index > 0)
						{
							var previousRow = (TreeNode) parentWidgets[index - 1];
							if (!previousRow.IsExpanded || previousRow.ChildNodesCount == 0)
							{
								SelectedRow = previousRow;
							}
							else
							{
								SelectedRow = (TreeNode) previousRow.ChildNodesGrid.Widgets[previousRow.ChildNodesCount - 1];
							}
						}
					}
				}
					break;
				case Keys.Down:
				{
					if (SelectedRow.IsExpanded && SelectedRow.ChildNodesCount > 0)
					{
						SelectedRow = (TreeNode) SelectedRow.ChildNodesGrid.Widgets[0];
					}
					else if (parentWidgets != null && index + 1 < parentWidgets.Count)
					{
						SelectedRow = (TreeNode) parentWidgets[index + 1];
					}
					else if (parentWidgets != null && index + 1 >= parentWidgets.Count)
					{
						var parentOfParent = SelectedRow.ParentNode.ParentNode;
						if (parentOfParent != null)
						{
							var parentIndex = parentOfParent.ChildNodesGrid.Widgets.IndexOf(SelectedRow.ParentNode);
							if (parentIndex + 1 < parentOfParent.ChildNodesCount)
							{
								SelectedRow = (TreeNode) parentOfParent.ChildNodesGrid.Widgets[parentIndex + 1];
							}
						}
					}
				}
					break;
			}
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

				SelectedRow = HoverRow;
			}
		}

		public override void OnDoubleClick(MouseButtons mb)
		{
			base.OnDoubleClick(mb);

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

				if (HoverRow.Mark.Visible && !HoverRow.Mark.Bounds.Contains(Desktop.MousePosition))
				{
					HoverRow.Mark.IsPressed = !HoverRow.Mark.IsPressed;
				}
			}
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			HoverRow = null;

			if (!Bounds.Contains(position))
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

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			HoverRow = null;
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
				var subNode = (TreeNode) widget;
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
			var bounds = Bounds;

			foreach (var rowInfo in _allNodes)
			{
				rowInfo.RowVisible = false;
			}

			RecursiveUpdateRowVisibility(this);

			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible)
				{
					rowInfo.RowBounds = new Rectangle(bounds.X, rowInfo.Bounds.Y, bounds.Width,
						rowInfo.Widget.GetRowHeight(0));
				}
			}
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();
			_rowInfosDirty = true;
		}

		public override void InternalRender(RenderContext context)
		{
			if (_rowInfosDirty)
			{
				UpdateRowInfos();
				_rowInfosDirty = false;
			}

			if (HoverRow != null && HoverRow != SelectedRow && SelectionHoverBackground != null)
			{
				context.Draw(SelectionHoverBackground, HoverRow.RowBounds);
			}

			if (SelectedRow != null && SelectedRow.RowVisible && SelectionBackground != null)
			{
				context.Draw(SelectionBackground, SelectedRow.RowBounds);
			}

			base.InternalRender(context);
		}

		protected override void UpdateMark()
		{
			if (!HasRoot)
			{
				return;
			}

			base.UpdateMark();
		}

		public void ApplyTreeStyle(TreeStyle style)
		{
			ApplyTreeNodeStyle(style);
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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTreeStyle(stylesheet.TreeStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.TreeStyles.Keys.ToArray();
		}
	}
}