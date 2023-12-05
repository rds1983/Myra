using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public class TreeView : Widget
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly List<TreeViewNode> _allNodes = new List<TreeViewNode>();
		private TreeViewNode _selectedRow;
		private bool _rowInfosDirty = true;

		internal List<TreeViewNode> AllNodes => _allNodes;

		private TreeViewNode HoverRow { get; set; }

		public TreeViewNode SelectedRow
		{
			get
			{
				return _selectedRow;
			}

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

		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		public event EventHandler SelectionChanged;

		public TreeView(string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			AcceptsKeyboardFocus = true;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			SetStyle(styleName);
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
							if (index == 0 && SelectedRow.ParentNode != null)
							{
								SelectedRow = SelectedRow.ParentNode;
							}
							else if (index > 0)
							{
								var previousRow = (TreeViewNode)parentWidgets[index - 1];
								if (!previousRow.IsExpanded || previousRow.ChildNodesCount == 0)
								{
									SelectedRow = previousRow;
								}
								else
								{
									SelectedRow = (TreeViewNode)previousRow.ChildNodesGrid.Widgets[previousRow.ChildNodesCount - 1];
								}
							}
						}
					}
					break;
				case Keys.Down:
					{
						if (SelectedRow.IsExpanded && SelectedRow.ChildNodesCount > 0)
						{
							SelectedRow = (TreeViewNode)SelectedRow.ChildNodesGrid.Widgets[0];
						}
						else if (parentWidgets != null && index + 1 < parentWidgets.Count)
						{
							SelectedRow = (TreeViewNode)parentWidgets[index + 1];
						}
						else if (parentWidgets != null && index + 1 >= parentWidgets.Count)
						{
							var parentOfParent = SelectedRow.ParentNode.ParentNode;
							if (parentOfParent != null)
							{
								var parentIndex = parentOfParent.ChildNodesGrid.Widgets.IndexOf(SelectedRow.ParentNode);
								if (parentIndex + 1 < parentOfParent.ChildNodesCount)
								{
									SelectedRow = (TreeViewNode)parentOfParent.ChildNodesGrid.Widgets[parentIndex + 1];
								}
							}
						}
					}
					break;
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return;
			}

			SetHoverRow(Desktop.TouchPosition.Value);

			if (HoverRow != null && HoverRow.RowVisible)
			{
				SelectedRow = HoverRow;
			}
		}

		public override void OnTouchDoubleClick()
		{
			base.OnTouchDoubleClick();

			if (HoverRow != null)
			{
				if (!HoverRow.RowVisible)
				{
					return;
				}

				if (HoverRow.Mark.Visible && !HoverRow.Mark.IsTouchInside)
				{
					HoverRow.Mark.DoClick();
				}
			}
		}

		private Rectangle BuildRowRect(TreeViewNode rowInfo)
		{
			var rowPos = ToLocal(rowInfo.ToGlobal(rowInfo.ActualBounds.Location));

			return new Rectangle(ActualBounds.Left, rowPos.Y, ActualBounds.Width, rowInfo.ContentHeight);
		}

		private void SetHoverRow(Point position)
		{
			if (!ContainsGlobalPoint(position))
			{
				return;
			}

			position = ToLocal(position);
			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible)
				{
					var rect = BuildRowRect(rowInfo);
					if (rect.Contains(position))
					{
						HoverRow = rowInfo;
						return;
					}
				}
			}
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			HoverRow = null;

			if (Desktop == null)
			{
				return;
			}

			SetHoverRow(Desktop.MousePosition);
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			HoverRow = null;
		}

		public TreeViewNode AddSubNode(Widget content)
		{
			var result = new TreeViewNode(this, StyleName)
			{
				Content = content
			};

			Grid.SetRow(result, Children.Count);

			Children.Add(result);

			return result;
		}

		public void RemoveAllSubNodes()
		{
			_allNodes.Clear();
			_selectedRow = null;
			HoverRow = null;
		}

		private bool Iterate(TreeViewNode node, Func<TreeViewNode, bool> action)
		{
			if (!action(node))
			{
				return false;
			}

			foreach (var widget in node.ChildNodesGrid.ChildrenCopy)
			{
				var subNode = (TreeViewNode)widget;
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
		public void Iterate(Func<TreeViewNode, bool> action)
		{
			foreach (TreeViewNode node in ChildrenCopy)
			{
				Iterate(node, action);
			}
		}

		private static void RecursiveUpdateRowVisibility(TreeViewNode tree)
		{
			tree.RowVisible = true;

			if (tree.IsExpanded)
			{
				foreach (var widget in tree.ChildNodesGrid.ChildrenCopy)
				{
					var TreeViewNode = (TreeViewNode)widget;
					RecursiveUpdateRowVisibility(TreeViewNode);
				}
			}
		}

		private void UpdateRowInfos()
		{
			foreach (var rowInfo in _allNodes)
			{
				rowInfo.RowVisible = false;
			}

			foreach (TreeViewNode node in ChildrenCopy)
			{
				RecursiveUpdateRowVisibility(node);
			}
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();
			_rowInfosDirty = true;
		}

		public override void InternalRender(RenderContext context)
		{
			if (_rowInfosDirty)
			{
				UpdateRowInfos();
				_rowInfosDirty = false;
			}
			if (SelectionBackground != null)
			{
				if (HoverRow != null && HoverRow != SelectedRow && SelectionHoverBackground != null)
				{
					var rect = BuildRowRect(HoverRow);
					SelectionHoverBackground.Draw(context, rect);
				}

				if (SelectedRow != null && SelectedRow.RowVisible)
				{
					var rect = BuildRowRect(SelectedRow);
					SelectionBackground.Draw(context, rect);
				}
			} else
			{
				if (HoverRow != null && SelectionHoverBackground != null)
				{
					var rect = BuildRowRect(HoverRow);
					SelectionHoverBackground.Draw(context, rect);
				}
			}

			base.InternalRender(context);
		}

		private static bool FindPath(Stack<TreeViewNode> path, TreeViewNode node)
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
		public void ExpandPath(TreeViewNode node)
		{
			var path = new Stack<TreeViewNode>();

			foreach (TreeViewNode childNode in Children)
			{
				path.Push(childNode);
			}

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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			base.InternalSetStyle(stylesheet, name);
			ApplyTreeViewStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}

		public void ApplyTreeViewStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var treeView = (TreeView)w;
			SelectionBackground = treeView.SelectionBackground;
			SelectionHoverBackground = treeView.SelectionHoverBackground;

			foreach(TreeViewNode node in treeView.Children)
			{
				AddSubNode(node.Content);
			}
		}
	}
}