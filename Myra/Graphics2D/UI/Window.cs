using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class Window : SingleItemContainer<Grid>
	{
		public enum DefaultModalResult
		{
			Ok,
			Cancel
		}

		private Point? _startPos;
		private readonly Grid _titleGrid;
		private readonly Grid _contentGrid;
		private readonly TextBlock _titleLabel;
		private readonly Button _closeButton;

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

		public Grid TitleGrid
		{
			get { return _titleGrid; }
		}

		public Grid ContentGrid
		{
			get { return _contentGrid; }
		}

		public int ModalResult { get; private set; }

		public event EventHandler Closed;

		public Window(WindowStyle style)
		{
			ModalResult = (int)DefaultModalResult.Cancel;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			Widget = new Grid
			{
				RowSpacing = 8
			};

			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_titleGrid = new Grid
			{
				ColumnSpacing = 8
			};

			_titleGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
			_titleGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_titleLabel = new TextBlock();
			_titleGrid.Children.Add(_titleLabel);

			_closeButton = new Button
			{
				GridPosition = {X = 1}
			};

			_closeButton.Up += (sender, args) =>
			{
				Close();
			};

			_titleGrid.Children.Add(_closeButton);

			Widget.Children.Add(_titleGrid);

			_contentGrid = new Grid {GridPosition = {Y = 1}};

			Widget.Children.Add(_contentGrid);

			if (style != null)
			{
				ApplyWindowStyle(style);
			}
		}

		public Window() : this(DefaultAssets.UIStylesheet.WindowStyle)
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

				var size = Measure(Desktop.Bounds.Size);

				XHint = (Desktop.Bounds.Width - size.X)/2;
				YHint = (Desktop.Bounds.Height - size.Y)/2;
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

			if (Desktop != null)
			{
				var mousePos = Desktop.MousePosition;
				if (_titleGrid.AbsoluteBounds.Contains(mousePos))
				{
					_startPos = mousePos;
				}
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TitleStyle != null)
			{
				_titleLabel.ApplyTextBlockStyle(style.TitleStyle);
			}

			if (style.CloseButtonStyle != null)
			{
				_closeButton.ApplyButtonStyle(style.CloseButtonStyle);
			}
		}

		public void ShowModal(Desktop desktop)
		{
			desktop.ModalWidget = this;
		}

		public void Close()
		{
			if (Desktop != null && Desktop.ModalWidget == this)
			{
				Desktop.ModalWidget = null;

				var ev = Closed;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
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

			okButton.Up += (sender, args) =>
			{
				w.ModalResult = (int) DefaultModalResult.Ok;
				w.Close();
			};

			buttonsGrid.Children.Add(okButton);

			var cancelButton = new Button
			{
				Text = "Cancel",
				GridPosition = {X = 1}
			};

			cancelButton.Up += (sender, args) =>
			{
				w.ModalResult = (int)DefaultModalResult.Cancel;
				w.Close();
			};

			buttonsGrid.Children.Add(cancelButton);

			w.ContentGrid.Children.Add(buttonsGrid);

			return w;
		}
	}
}