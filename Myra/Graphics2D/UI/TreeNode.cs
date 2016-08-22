using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class TreeNode : SingleItemContainer<Grid>
	{
		internal class RowInfo
		{
			public Rectangle Bounds;
			public TreeNode TreeNode { get; private set; }
			public bool Visible { get; set; }

			public RowInfo(TreeNode treeNode)
			{
				TreeNode = treeNode;
			}
		}

		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly Button _mark;
		private readonly TextBlock _label;
		private readonly RowInfo _thisRowInfo;

		public bool IsExpanded
		{
			get { return _childNodesGrid.Visible; }
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

		internal Grid ChildNodesGrid
		{
			get { return _childNodesGrid; }
		}

		public TreeStyle TreeStyle { get; private set; }

		internal RowInfo ThisRow
		{
			get { return _thisRowInfo; }
		}

		public TreeNode(TreeStyle style, Tree topTree)
		{
			_topTree = topTree;
			_thisRowInfo = new RowInfo(this);

			if (_topTree != null)
			{
				_topTree.RowInfos.Add(_thisRowInfo);
			}

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Widget = new Grid();

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_mark = new Button(null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};

			_mark.Up += MarkOnUp;

			Widget.Children.Add(_mark);

			_label = new TextBlock(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				GridPosition =
				{
					X = 1
				}
			};

			Widget.Children.Add(_label);

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid
			{
				GridPosition =
				{
					X = 1,
					Y = 1
				},
				Visible = false
			};

			Widget.Children.Add(_childNodesGrid);

			if (style != null)
			{
				ApplyTreeNodeStyle(style);
			}

			UpdateMark();
		}

		private void MarkOnUp(object sender, GenericEventArgs<MouseButtons> genericEventArgs)
		{
			_childNodesGrid.Visible = !_childNodesGrid.Visible;
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
			var result = new TreeNode(TreeStyle, _topTree ?? (Tree)this)
			{
				Text = text,
				GridPosition = { Y = _childNodesGrid.Children.Count },
			};

			_childNodesGrid.Children.Add(result);
			_childNodesGrid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			UpdateMark();

			return result;
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

			_mark.WidthHint = null;
			_mark.HeightHint = null;
			if (style.MarkStyle != null)
			{
				if (style.MarkStyle.Image != null)
				{
					_mark.WidthHint = style.MarkStyle.Image.Size.X;
					_mark.HeightHint = style.MarkStyle.Image.Size.Y;
				}

				if (style.MarkStyle.PressedImage != null)
				{
					if (_mark.WidthHint != null && style.MarkStyle.PressedImage.Size.X > _mark.WidthHint.Value)
					{
						_mark.WidthHint = style.MarkStyle.PressedImage.Size.X;
					}

					if (_mark.HeightHint != null && style.MarkStyle.PressedImage.Size.Y > _mark.HeightHint.Value)
					{
						_mark.HeightHint = style.MarkStyle.PressedImage.Size.Y;
					}
				}
			}
		}
	}
}