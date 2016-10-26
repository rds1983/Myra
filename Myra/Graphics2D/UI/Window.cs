using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class Window : SingleItemContainer<Grid>
	{
		private Point? _startPos;
		private readonly Grid _titleGrid;
		private readonly Grid _contentGrid;
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
			get { return _titleGrid; }
		}

		public Grid ContentGrid
		{
			get { return _contentGrid; }
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
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			Grid = new Grid
			{
				RowSpacing = 8
			};

			Grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_titleGrid = new Grid
			{
				ColumnSpacing = 8
			};

			_titleGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
			_titleGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_titleLabel = new TextBlock();
			_titleGrid.Children.Add(_titleLabel);

			var titleQuit = new Button
			{
				Text = "x",
				GridPosition = {X = 1}
			};

			_titleGrid.Children.Add(titleQuit);

			Grid.Children.Add(_titleGrid);

			_contentGrid = new Grid
			{
				GridPosition = {Y = 1}
			};
			Grid.Children.Add(_contentGrid);

			if (style != null)
			{
				ApplyWindowStyle(style);
			}
		}

		public Window() : this(Stylesheet.Current.WindowStyle)
		{
		}

		public override void OnDesktopChanging()
		{
			base.OnDesktopChanging();

			if (Desktop != null)
			{
				Desktop.MouseMoved -= DesktopOnMouseMoved;
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			if (Desktop != null)
			{
				Desktop.MouseMoved += DesktopOnMouseMoved;

				XHint = (Desktop.Bounds.Width - Bounds.Width)/2;
				YHint = (Desktop.Bounds.Height - Bounds.Height)/2;
			}
		}

		private void DesktopOnMouseMoved(object sender, GenericEventArgs<Point> genericEventArgs)
		{

			if (_startPos == null)
			{
				return;
			}

			var position = genericEventArgs.Data;

			var delta = new Point(position.X - _startPos.Value.X,
				position.Y - _startPos.Value.Y);

			var right = delta.X + Bounds.Width;
			if (Parent != null)
			{
				if (right > Parent.Bounds.Right)
				{
					delta.X = Parent.Bounds.Right - Bounds.Width;
				}
			}
			else if (Desktop != null)
			{
				if (right > Desktop.Bounds.Right)
				{
					delta.X = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			var bottom = delta.Y + Bounds.Height;
			if (Parent != null)
			{
				if (bottom > Parent.Bounds.Bottom)
				{
					delta.Y = Parent.Bounds.Bottom - Bounds.Height;
				}
			}
			else if (Desktop != null)
			{
				if (bottom > Desktop.Bounds.Bottom)
				{
					delta.Y = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			XHint += delta.X;

			if (XHint < 0)
			{
				XHint = 0;
			}

			if (Desktop != null)
			{
				if (XHint + Bounds.Width > Desktop.Bounds.Right)
				{
					XHint = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			YHint += delta.Y;

			if (YHint < 0)
			{
				YHint = 0;
			}

			if (Desktop != null)
			{
				if (YHint + Bounds.Height > Desktop.Bounds.Bottom)
				{
					YHint = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			_startPos = position;
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
				_startPos = mousePos;
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);
		}

		public void ShowModal(Desktop desktop)
		{
			desktop.ModalWidget = this;
		}

		public static Window CreateMessageBox(string title, string message)
		{
			var w = new Window
			{
				Title = "Quit",
				WidthHint = 200,
			};

			var messageLabel = new TextBlock
			{
				Text = message
			};

			w.ContentGrid.RowsProportions.Add(new Grid.Proportion());
			w.ContentGrid.RowsProportions.Add(new Grid.Proportion());

			w.ContentGrid.Children.Add(messageLabel);

			var buttonsGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right,
				GridPosition = {Y = 1}
			};

			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion());
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion());

			var okButton = new Button
			{
				Text = "Ok"
			};
			buttonsGrid.Children.Add(okButton);

			var cancelButton = new Button
			{
				Text = "Cancel",
				GridPosition = {X = 1}
			};
			buttonsGrid.Children.Add(cancelButton);

			w.ContentGrid.Children.Add(buttonsGrid);

			return w;
		}
	}
}