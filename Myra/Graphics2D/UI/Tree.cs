using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI.Styles;
using Myra.Input;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class Tree : TreeNode
	{
		private readonly List<RowInfo> _rowInfos = new List<RowInfo>();
		private RowInfo _selectedRow;

		public Drawable RowOverBackground { get; set; }
		public Drawable RowSelectionBackground { get; set; }

		internal List<RowInfo> RowInfos
		{
			get { return _rowInfos; }
		}

		private RowInfo HoverRow { get; set; }

		private RowInfo SelectedRow
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

		public TreeNode SelectedTreeNode
		{
			get { return _selectedRow != null ? _selectedRow.TreeNode : null; }
		}

		public event EventHandler SelectionChanged; 

		public Tree(TreeStyle style): base(style, null)
		{
			InputAPI.MouseMoved += InputOnMouseMoved;
			InputAPI.MouseDown += InputOnMouseDown;

			if (style != null)
			{
				ApplyTreeStyle(style);
			}
		}

		public Tree() : this(Stylesheet.Current.TreeStyle)
		{
		}

		private void InputOnMouseDown(object sender, GenericEventArgs<MouseButtons> genericEventArgs)
		{
			if (genericEventArgs.Data != MouseButtons.Left)
			{
				return;
			}

			if (HoverRow != null)
			{
				if (!HoverRow.Visible)
				{
					return;
				}

				if (HoverRow.TreeNode.Mark.Bounds.Contains(InputAPI.MousePosition))
				{
					return;
				}

				SelectedRow = SelectedRow != HoverRow ? HoverRow : null;
			}
		}

		private void InputOnMouseMoved(object sender, GenericEventArgs<Point> genericEventArgs)
		{
			HoverRow = null;

			if (!ClientBounds.Contains(genericEventArgs.Data))
			{
				return;
			}

			foreach (var rowInfo in _rowInfos)
			{
				if (rowInfo.Visible && rowInfo.Bounds.Contains(genericEventArgs.Data))
				{
					HoverRow = rowInfo;
					return;
				}
			}
		}

		public override void RemoveAllSubNodes()
		{
			base.RemoveAllSubNodes();

			_rowInfos.Clear();
			_rowInfos.Add(ThisRow);
		}

		private static void RecursiveUpdateRowVisibility(TreeNode tree)
		{
			tree.ThisRow.Visible = true;

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

			foreach (var rowInfo in _rowInfos)
			{
				rowInfo.Visible = false;
			}

			RecursiveUpdateRowVisibility(this);

			foreach (var rowInfo in _rowInfos)
			{
				if (rowInfo.Visible)
				{
					rowInfo.Bounds = new Rectangle(bounds.X, rowInfo.TreeNode.ClientBounds.Y, bounds.Width,
						rowInfo.TreeNode.Widget.GetRowHeight(0));
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
			if (SelectedRow != null && SelectedRow.Visible && RowSelectionBackground != null)
			{
				RowSelectionBackground.Draw(batch, SelectedRow.Bounds);
			}

			if (HoverRow != null && HoverRow.Visible && RowOverBackground != null && HoverRow != SelectedRow)
			{
				RowOverBackground.Draw(batch, HoverRow.Bounds);
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