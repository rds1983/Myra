using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class Window: SingleItemContainer<Grid>
	{
		private Point? _startPos;
		private readonly Grid _titleGrid;
		private readonly TextBlock _titleLabel;
		private Widget _widget;

		public string Title
		{
			get { return _titleLabel.Text; }

			set { _titleLabel.Text = value; }
		}

		public BitmapFont TitleFont
		{
			get { return _titleLabel.Font; }
			set { _titleLabel.Font = value; }
		}

		public Color TitleTextColor
		{
			get { return _titleLabel.TextColor; }
			set { _titleLabel.TextColor = value; }
		}

		private Grid Grid
		{
			get { return base.Widget; }
			set { base.Widget = value; }
		}

		public Grid TitleGrid
		{
			get
			{
				return _titleGrid;
			}
		}

		public new Widget Widget
		{
			get { return _widget; }

			set
			{
				if (_widget == value)
				{
					return;
				}

				if (_widget != null)
				{
					Grid.Children.Remove(_widget);
				}

				_widget = value;

				if (_widget != null)
				{
					_widget.GridPosition.Y = 1;
					Grid.Children.Add(_widget);
				}
			}
		}

		public Window(WindowStyle style)
		{
			Grid = new Grid();

			Grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part, 1.0f));

			_titleGrid = new Grid();

			_titleLabel = new TextBlock();
			_titleGrid.Children.Add(_titleLabel);

			Grid.Children.Add(_titleGrid);

			if (style != null)
			{
				ApplyWindowStyle(style);
			}
		}

		public Window(): this(Stylesheet.Current.WindowStyle)
		{
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (_startPos == null)
			{
				return;
			}

			var newPos = new Point(position.X - _startPos.Value.X,
				position.Y - _startPos.Value.Y);

			if (newPos.X < 0)
			{
				newPos.X = 0;
			}

			var right = newPos.X + Bounds.Width;
			if (Parent != null)
			{
				if (right > Parent.Bounds.Right)
				{
					newPos.X = Parent.Bounds.Right - Bounds.Width;
				}
			} else if (Desktop != null)
			{
				if (right > Desktop.Bounds.Right)
				{
					newPos.X = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			if (newPos.Y < 0)
			{
				newPos.Y = 0;
			}

			var bottom = newPos.Y + Bounds.Height;
			if (Parent != null)
			{
				if (bottom > Parent.Bounds.Bottom)
				{
					newPos.Y = Parent.Bounds.Bottom - Bounds.Height;
				}
			}
			else if (Desktop != null)
			{
				if (bottom > Desktop.Bounds.Bottom)
				{
					newPos.Y = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			XHint = newPos.X;
			YHint = newPos.Y;
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			_startPos = null;
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			var mousePos = Desktop.MousePosition;
			if (_titleGrid.Bounds.Contains(mousePos))
			{
				mousePos.X -= _titleGrid.Bounds.X;
				mousePos.Y -= _titleGrid.Bounds.Y;
				_startPos = mousePos;
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);
		}
	}
}
