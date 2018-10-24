using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class TreeNode : GridBased
	{
		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly ImageButton _mark;
		private readonly TextBlock _label;
		private bool _hasRoot = true;

		public bool IsExpanded
		{
			get { return !HasRoot || _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		public ImageButton Mark
		{
			get { return _mark; }
		}

		public string Text
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		public SpriteFont Font
		{
			get { return _label.Font; }
			set { _label.Font = value; }
		}

		public Color TextColor
		{
			get { return _label.TextColor; }
			set { _label.TextColor = value; }
		}

		public int ChildNodesCount
		{
			get { return _childNodesGrid.ChildCount; }
		}

		internal Grid ChildNodesGrid
		{
			get { return _childNodesGrid; }
		}

		internal Rectangle RowBounds { get; set; }

		internal bool RowVisible { get; set; }

		public TreeNode ParentNode { get; internal set; }

		public TreeStyle TreeStyle { get; private set; }

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

		public TreeNode(TreeStyle style, Tree topTree): base(style)
		{
			_topTree = topTree;

			ColumnSpacing = 2;
			RowSpacing = 2;

			if (_topTree != null)
			{
				_topTree.AllNodes.Add(this);
			}

			_mark = new ImageButton((ImageButtonStyle)null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};

			_mark.Down += MarkOnDown;
			_mark.Up += MarkOnUp;

			Widgets.Add(_mark);

			_label = new TextBlock
			{
				GridPositionX = 1
			};

			Widgets.Add(_label);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			RowsProportions.Add(new Proportion(ProportionType.Auto));
			RowsProportions.Add(new Proportion(ProportionType.Auto));

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid((GridStyle)null)
			{
				Visible = false,
				GridPositionX = 1,
				GridPositionY =  1
			};

			Widgets.Add(_childNodesGrid);

			if (style != null)
			{
				ApplyTreeNodeStyle(style);
			}

			UpdateMark();
			UpdateHasRoot();
		}

		private void UpdateHasRoot()
		{
			if (_hasRoot)
			{
				_mark.Visible = true;
				_label.Visible = true;
				_childNodesGrid.Visible = _mark.IsPressed;
			}
			else
			{
				_mark.Visible = false;
				_label.Visible = false;
				_childNodesGrid.Visible = true;
			}
		}

		private void MarkOnDown(object sender, EventArgs eventArgs)
		{
			_childNodesGrid.Visible = true;
		}

		private void MarkOnUp(object sender, EventArgs args)
		{
			_childNodesGrid.Visible = false;
		}

		private void UpdateMark()
		{
			_mark.ImageVisible = _childNodesGrid.Widgets.Count > 0;
		}

		public virtual void RemoveAllSubNodes()
		{
			_childNodesGrid.Widgets.Clear();
			_childNodesGrid.RowsProportions.Clear();

			UpdateMark();
		}

		public TreeNode AddSubNode(string text)
		{
			var result = new TreeNode(TreeStyle, _topTree ?? (Tree) this)
			{
				Text = text,
				GridPositionY = _childNodesGrid.Widgets.Count,
				ParentNode = this
			};

			_childNodesGrid.Widgets.Add(result);
			_childNodesGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			UpdateMark();

			return result;
		}

		public TreeNode GetSubNode(int index)
		{
			return (TreeNode) _childNodesGrid.Widgets[index];
		}

		public void RemoveSubNode(TreeNode subNode)
		{
			_childNodesGrid.Widgets.Remove(subNode);
			if (_topTree.SelectedRow == subNode)
			{
				_topTree.SelectedRow = null;
			}
		}

		public void RemoveSubNodeAt(int index)
		{
			var subNode = _childNodesGrid.Widgets[index];
			_childNodesGrid.Widgets.RemoveAt(index);
			if (_topTree.SelectedRow == subNode)
			{
				_topTree.SelectedRow = null;
			}
		}

		public void ApplyTreeNodeStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.MarkStyle != null)
			{
				_mark.ApplyImageButtonStyle(style.MarkStyle);
				_label.ApplyTextBlockStyle(style.LabelStyle);
			}

			TreeStyle = style;
		}
	}
}