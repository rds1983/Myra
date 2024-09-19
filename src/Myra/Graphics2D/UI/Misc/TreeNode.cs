using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using FontStashSharp;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	[Obsolete("Use TreeView")]
	public class TreeNode : Widget
	{
		private readonly SingleItemLayout<Grid> _layout;
		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly ToggleButton _mark;
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

		public ToggleButton Mark
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
			get { return _layout.Child; }
		}

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
			_layout = new SingleItemLayout<Grid>(this)
			{
				Child = new Grid
				{
					ColumnSpacing = 2,
					RowSpacing = 2,
				}
			};
			ChildrenLayout = _layout;


			_topTree = topTree;

			if (_topTree != null)
			{
				_topTree.AllNodes.Add(this);
			}

			_mark = new ToggleButton(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Content = new Image()
			};

			_mark.PressedChanged += (s, a) =>
			{
				_childNodesGrid.Visible = _mark.IsPressed;
			};

			Grid.Widgets.Add(_mark);

			_label = new Label();
			Grid.SetColumn(_label, 1);

			Grid.Widgets.Add(_label);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			Grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			Grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			Grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid()
			{
				Visible = false,
			};
			Grid.SetColumn(_childNodesGrid, 1);
			Grid.SetRow(_childNodesGrid, 1);

			Grid.Widgets.Add(_childNodesGrid);

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
			var result = new TreeNode(_topTree ?? (Tree)this, StyleName)
			{
				Text = text,
				ParentNode = this
			};
			Grid.SetRow(result, _childNodesGrid.Widgets.Count);

			_childNodesGrid.Widgets.Add(result);
			_childNodesGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			UpdateMark();

			return result;
		}

		public TreeNode GetSubNode(int index)
		{
			return (TreeNode)_childNodesGrid.Widgets[index];
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
				_mark.ApplyButtonStyle(style.MarkStyle);
				if (style.MarkStyle.ImageStyle != null)
				{
					var image = (Image)_mark.Content;
					image.ApplyPressableImageStyle(style.MarkStyle.ImageStyle);
				}


				_label.ApplyLabelStyle(style.LabelStyle);
			}

			TreeStyle = style;

			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTreeNodeStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}
	}
}