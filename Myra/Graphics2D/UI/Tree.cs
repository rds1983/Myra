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

		public Drawable RowOverBackground { get; set; }
		public Drawable RowSelectionBackground { get; set; }

		public List<TreeNode> AllNodes
		{
			get { return _allNodes; }
		}

		private TreeNode HoverRow { get; set; }

		public TreeNode SelectedRow
		{
			get { return _selectedRow; }

			private set
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

		public event EventHandler SelectionChanged; 

		public Tree(TreeStyle style): base(style, null)
		{
			if (style != null)
			{
				ApplyTreeStyle(style);
			}
		}

		public Tree() : this(Stylesheet.Current.TreeStyle)
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

				if (HoverRow.Mark.Bounds.Contains(Desktop.MousePosition))
				{
					return;
				}

				SelectedRow = SelectedRow != HoverRow ? HoverRow : null;
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

		public override void RemoveAllSubNodes()
		{
			base.RemoveAllSubNodes();

			_allNodes.Clear();
			_allNodes.Add(this);

			_selectedRow = null;
			HoverRow = null;
		}

		private static void RecursiveUpdateRowVisibility(TreeNode tree)
		{
			tree.RowVisible = true;

			if (tree.IsExpanded)
			{
				foreach (var widget in tree.ChildNodesGrid.Children)
				{
					var TreeNode = (TreeNode) widget;
					RecursiveUpdateRowVisibility(TreeNode);
				}
			}
		}

		private void UpdateRowInfos()
		{
			var bounds = ClientBounds;

			foreach (var rowInfo in _allNodes)
			{
				rowInfo.RowVisible = false;
			}

			RecursiveUpdateRowVisibility(this);

			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible)
				{
					rowInfo.RowBounds = new Rectangle(bounds.X, rowInfo.ClientBounds.Y, bounds.Width,
						rowInfo.Widget.GetRowHeight(0));
				}
			}
		}

		public override void FireLocationChanged()
		{
			base.FireLocationChanged();

			UpdateRowInfos();
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			UpdateRowInfos();
		}

		public override void InternalRender(SpriteBatch batch)
		{
			if (SelectedRow != null && SelectedRow.RowVisible && RowSelectionBackground != null)
			{
				RowSelectionBackground.Draw(batch, SelectedRow.RowBounds);
			}

			if (HoverRow != null && HoverRow.RowVisible && RowOverBackground != null && HoverRow != SelectedRow)
			{
				RowOverBackground.Draw(batch, HoverRow.RowBounds);
			}

			base.InternalRender(batch);
		}

		public void ApplyTreeStyle(TreeStyle style)
		{
			ApplyTreeNodeStyle(style);

			RowOverBackground = style.RowOverBackground;
			RowSelectionBackground = style.RowSelectionBackground;
		}
	}
}