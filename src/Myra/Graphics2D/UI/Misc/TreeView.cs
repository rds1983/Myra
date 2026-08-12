using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Events;
using System.Collections;


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
	/// <summary>
	/// A hierarchical tree view widget that displays nodes in an expandable/collapsible tree structure.
	/// </summary>
	public class TreeView : Widget, ITreeViewNode
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly List<TreeViewNode> _allNodes = new List<TreeViewNode>();
		private TreeViewNode _selectedNode;
		private bool _rowInfosDirty = true;

		internal List<TreeViewNode> AllNodes => _allNodes;

		internal TreeStyle TreeStyle { get; set; }

		/// <summary>
		/// Gets the number of child nodes directly under this tree view.
		/// </summary>
		public int SubNodesCount => Children.Count;

		/// <summary>
		/// Gets the number of top-level child nodes.
		/// </summary>
		[Obsolete("Use SubNodesCount")]
		public int ChildNodesCount => Children.Count;

		/// <summary>
		/// Gets the total number of nodes in the tree (including all nested nodes).
		/// </summary>
		public int TotalNodesCount => _allNodes.Count;

		internal TreeViewNode HoverRow { get; set; }

		/// <summary>
		/// Gets or sets the currently selected node.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public TreeViewNode SelectedNode
		{
			get
			{
				return _selectedNode;
			}

			set
			{
				if (value == _selectedNode)
				{
					return;
				}

				_selectedNode = value;

				var ev = SelectionChanged;
				if (ev != null)
				{
					ev(this, new MyraEventArgs(InputEventType.SelectionChanged));
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background of selected nodes.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of hovered nodes.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Occurs when the selected node changes.
		/// </summary>
		public event MyraEventHandler SelectionChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeView"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public TreeView(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			AcceptsKeyboardFocus = true;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeView"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public TreeView(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Handles keyboard input for tree navigation (Up, Down, and Enter keys).
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (SelectedNode == null)
			{
				return;
			}

			int index = 0;
			IList<Widget> parentWidgets = null;
			if (SelectedNode.ParentNode != null)
			{
				parentWidgets = SelectedNode.ParentNode.ChildNodesGrid.Widgets;
				index = parentWidgets.IndexOf(SelectedNode);
				if (index == -1)
				{
					return;
				}
			}

			switch (k)
			{
				case Keys.Enter:
					SelectedNode.IsExpanded = !SelectedNode.IsExpanded;
					break;
				case Keys.Up:
					{
						if (parentWidgets != null)
						{
							if (index == 0 && SelectedNode.ParentNode != null)
							{
								SelectedNode = SelectedNode.ParentNode;
							}
							else if (index > 0)
							{
								var previousRow = (TreeViewNode)parentWidgets[index - 1];
								if (!previousRow.IsExpanded || previousRow.ChildNodesCount == 0)
								{
									SelectedNode = previousRow;
								}
								else
								{
									SelectedNode = (TreeViewNode)previousRow.ChildNodesGrid.Widgets[previousRow.ChildNodesCount - 1];
								}
							}
						}
					}
					break;
				case Keys.Down:
					{
						if (SelectedNode.IsExpanded && SelectedNode.ChildNodesCount > 0)
						{
							SelectedNode = (TreeViewNode)SelectedNode.ChildNodesGrid.Widgets[0];
						}
						else if (parentWidgets != null && index + 1 < parentWidgets.Count)
						{
							SelectedNode = (TreeViewNode)parentWidgets[index + 1];
						}
						else if (parentWidgets != null && index + 1 >= parentWidgets.Count)
						{
							var parentOfParent = SelectedNode.ParentNode.ParentNode;
							if (parentOfParent != null)
							{
								var parentIndex = parentOfParent.ChildNodesGrid.Widgets.IndexOf(SelectedNode.ParentNode);
								if (parentIndex + 1 < parentOfParent.ChildNodesCount)
								{
									SelectedNode = (TreeViewNode)parentOfParent.ChildNodesGrid.Widgets[parentIndex + 1];
								}
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// Handles touch down events and selects the node at the touch position.
		/// </summary>
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
				SelectedNode = HoverRow;
			}
		}

		/// <summary>
		/// Handles touch double-click events and toggles node expansion if clicked on the mark.
		/// </summary>
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

		/// <summary>
		/// Handles mouse movement and updates the hover state of nodes.
		/// </summary>
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

		/// <summary>
		/// Handles mouse leaving the tree view and clears the hover state.
		/// </summary>
		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			HoverRow = null;
		}

		/// <summary>
		/// Adds a new top-level node to the tree with the specified content.
		/// </summary>
		/// <param name="content">The widget to display as the node's content.</param>
		/// <returns>The newly created tree node.</returns>
		public TreeViewNode AddSubNode(Widget content)
		{
			var result = new TreeViewNode(this)
			{
				Content = content
			};

			Grid.SetRow(result, Children.Count);

			Children.Add(result);

			return result;
		}

		/// <summary>
		/// Gets a top-level node by its index.
		/// </summary>
		/// <param name="index">The zero-based index of the node.</param>
		/// <returns>The tree node at the specified index.</returns>
		public TreeViewNode GetSubNode(int index) => (TreeViewNode)Children[index];

		/// <summary>
		/// Gets a node by its absolute index (across all levels of the tree).
		/// </summary>
		/// <param name="index">The zero-based absolute index.</param>
		/// <returns>The tree node at the specified absolute index.</returns>
		public TreeViewNode GetNodeByAbsoluteIndex(int index) => _allNodes[index];

		/// <summary>
		/// Removes all nodes from the tree.
		/// </summary>
		public void RemoveAllSubNodes()
		{
			HoverRow = SelectedNode = null;
			Children.Clear();
			_allNodes.Clear();
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

		/// <summary>
		/// Arranges the tree view and marks row information as needing update.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();
			_rowInfosDirty = true;
		}

		/// <summary>
		/// Renders the tree view including selection and hover backgrounds for rows.
		/// </summary>
		/// <param name="context">The render context used for drawing.</param>
		public override void InternalRender(RenderContext context)
		{
			if (_rowInfosDirty)
			{
				UpdateRowInfos();
				_rowInfosDirty = false;
			}
			if (SelectionBackground != null)
			{
				if (HoverRow != null && HoverRow != SelectedNode && SelectionHoverBackground != null)
				{
					var rect = BuildRowRect(HoverRow);
					SelectionHoverBackground.Draw(context, rect);
				}

				if (SelectedNode != null && SelectedNode.RowVisible)
				{
					var rect = BuildRowRect(SelectedNode);
					SelectionBackground.Draw(context, rect);
				}
			}
			else
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

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.TreeStyles;

		/// <summary>
		/// Applies the specified widget style to this tree view.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var treeStyle = (TreeStyle)style;

			TreeStyle = new TreeStyle(treeStyle);
			SelectionBackground = treeStyle.SelectionBackground;
			SelectionHoverBackground = treeStyle.SelectionHoverBackground;
		}

		/// <summary>
		/// Finds the first node that matches the specified predicate.
		/// </summary>
		/// <param name="predicate">A function to test each node; returns true if the node matches.</param>
		/// <returns>The first matching node, or null if no nodes match.</returns>
		public TreeViewNode FindNode(Func<TreeViewNode, bool> predicate)
		{
			foreach (var node in AllNodes)
			{
				if (predicate(node))
				{
					return node;
				}
			}

			return null;
		}

		/// <summary>
		/// Copies the properties and child nodes from another tree view widget.
		/// </summary>
		/// <param name="w">The source tree view widget to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var treeView = (TreeView)w;
			SelectionBackground = treeView.SelectionBackground;
			SelectionHoverBackground = treeView.SelectionHoverBackground;

			foreach (TreeViewNode node in treeView.Children)
			{
				AddSubNode(node.Content);
			}
		}
	}
}