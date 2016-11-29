using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class TreeNode : SingleItemContainer<Grid>
	{
		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly Button _mark;
		private readonly TextBlock _label;
		private bool _hasRoot = true;

		public bool IsExpanded
		{
			get { return !HasRoot || _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		public Button Mark
		{
			get { return _mark; }
		}

		public TextBlock Label
		{
			get { return _label; }
		}

		public string Text
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		public BitmapFont Font
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

		public TreeStyle TreeStyle { get; private set; }

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

		public TreeNode(TreeStyle style, Tree topTree)
		{
			_topTree = topTree;

			if (_topTree != null)
			{
				_topTree.AllNodes.Add(this);
			}

			Widget = new Grid();

			_mark = new Button(null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};

			_mark.Down += MarkOnDown;
			_mark.Up += MarkOnUp;

			Widget.Children.Add(_mark);

			_label = new TextBlock(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				GridPosition = { X = 1 }
			};

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			Widget.Children.Add(_label);

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid
			{
				Visible = false,
				GridPosition =
				{
					X = 1,
					Y = 1
				}
			};

			Widget.Children.Add(_childNodesGrid);

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
			_mark.ImageVisible = _childNodesGrid.Children.Count > 0;
		}

		public virtual void RemoveAllSubNodes()
		{
			_childNodesGrid.Children.Clear();
			_childNodesGrid.RowsProportions.Clear();

			UpdateMark();
		}

		public TreeNode AddSubNode(string text)
		{
			var result = new TreeNode(TreeStyle, _topTree ?? (Tree) this)
			{
				Text = text,
				GridPosition = {Y = _childNodesGrid.Children.Count}
			};

			_childNodesGrid.Children.Add(result);
			_childNodesGrid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			UpdateMark();

			return result;
		}

		public TreeNode GetSubNode(int index)
		{
			return (TreeNode) _childNodesGrid.Children[index];
		}

		public void RemoveSubNode(int index)
		{
			_childNodesGrid.Children.RemoveAt(index);
		}

		public void ApplyTreeNodeStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.MarkStyle != null)
			{
				_mark.ApplyButtonStyle(style.MarkStyle);
			}

			if (style.LabelStyle != null)
			{
				_label.ApplyTextBlockStyle(style.LabelStyle);
			}

			TreeStyle = style;
		}
	}
}