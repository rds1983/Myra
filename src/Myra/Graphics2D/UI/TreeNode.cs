using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using FontStashSharp;

#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TreeNode : SingleItemContainer<Grid>
	{
		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly ImageButton _mark;
		private readonly Label _label;

		public bool IsExpanded
		{
			get { return _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		public Label Label
		{
			get
			{
				return _label;
			}
		}

		public ImageButton Mark
		{
			get { return _mark; }
		}

		public Grid ChildNodesGrid
		{
			get { return _childNodesGrid; }
		}

		public string Text
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		public SpriteFontBase Font
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
			get { return _childNodesGrid.Widgets.Count; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public Grid Grid
		{
			get { return InternalChild; }
		}

		internal Rectangle RowBounds { get; set; }

		internal bool RowVisible { get; set; }

		public TreeNode ParentNode { get; internal set; }

		public TreeStyle TreeStyle { get; private set; }

		[Category("Appearance")]
		public IBrush SelectionBackground
		{
			get; set;
		}

		[Category("Appearance")]
		public IBrush SelectionHoverBackground
		{
			get; set;
		}

		public TreeNode(Tree topTree, string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Grid();

			_topTree = topTree;

			InternalChild.ColumnSpacing = 2;
			InternalChild.RowSpacing = 2;

			if (_topTree != null)
			{
				_topTree.AllNodes.Add(this);
			}

			_mark = new ImageButton(null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};

			_mark.PressedChanged += (s, a) =>
			{
				_childNodesGrid.Visible = _mark.IsPressed;
			};

			InternalChild.Widgets.Add(_mark);

			_label = new Label
			{
				GridColumn = 1
			};

			InternalChild.Widgets.Add(_label);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));
			InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid()
			{
				Visible = false,
				GridColumn = 1,
				GridRow =  1
			};

			InternalChild.Widgets.Add(_childNodesGrid);

			SetStyle(styleName);

			UpdateMark();
		}

		private void MarkOnUp(object sender, EventArgs args)
		{
			_childNodesGrid.Visible = false;
		}

		protected virtual void UpdateMark()
		{
			_mark.Visible = _childNodesGrid.Widgets.Count > 0;
		}

		public virtual void RemoveAllSubNodes()
		{
			_childNodesGrid.Widgets.Clear();
			_childNodesGrid.RowsProportions.Clear();

			UpdateMark();
		}

		public TreeNode AddSubNode(string text)
		{
			var result = new TreeNode(_topTree ?? (Tree) this, StyleName)
			{
				Text = text,
				GridRow = _childNodesGrid.Widgets.Count,
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
			if (_topTree != null && _topTree.SelectedRow == subNode)
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
				_label.ApplyLabelStyle(style.LabelStyle);
			}

			TreeStyle = style;

			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTreeNodeStyle(stylesheet.TreeStyles[name]);
		}
	}
}