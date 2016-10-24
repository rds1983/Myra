using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class ScrollPane<T> : SingleItemContainer<T> where T: Widget
	{
		private Orientation _scrollbarOrientation;
		private bool _horizontalScrollbarVisible, _verticalScrollbarVisible;
		private Rectangle _horizontalScrollbarFrame, _horizontalScrollbarThumb;
		private Rectangle _verticalScrollbarFrame, _verticalScrollbarThumb;
		private Point _scrollPosition, _origin;
		private int? _startBoundsPos;
		private int _horizontalMaximum, _verticalMaximum;

		public Point ScrollPosition
		{
			get { return _scrollPosition; }
		}

		public Drawable HorizontalScrollBackground { get; set; }
		public Drawable HorizontalScrollKnob { get; set; }
		public Drawable VerticalScrollBackground { get; set; }
		public Drawable VerticalScrollKnob { get; set; }

		public override bool CanFocus
		{
			get { return true; }
		}

		public ScrollPane(ScrollAreaStyle style)
		{
			_horizontalScrollbarVisible = _verticalScrollbarVisible = false;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			if (style != null)
			{
				ApplyScrollPaneStyle(style);
			}
		}

		public ScrollPane() : this(Stylesheet.Current.ScrollAreaStyle)
		{
		}

		private void UpdateWidgetLocation()
		{

			if (Widget != null)
			{
				Widget.Location = Location - _origin;
			}
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (!_startBoundsPos.HasValue) return;
			var origin = _origin;

			if (_scrollbarOrientation == Orientation.Horizontal)
			{
				var delta = position.X - Bounds.X - _startBoundsPos.Value;
				if (delta < 0)
				{
					delta = 0;
				}

				if (delta > _horizontalMaximum)
				{
					delta = _horizontalMaximum;
				}

				var prop = (float)delta/_horizontalMaximum;

				_scrollPosition.X = delta;
				_horizontalScrollbarThumb.X = Bounds.X + delta;

				origin.X = (int)(prop*
				           (Widget.Bounds.Width - Bounds.Width + (_verticalScrollbarVisible ? _verticalScrollbarThumb.Width : 0)));
			}
			else
			{
				var delta = position.Y - Bounds.Y - _startBoundsPos.Value;
				if (delta < 0)
				{
					delta = 0;
				}

				if (delta > _verticalMaximum)
				{
					delta = _verticalMaximum;
				}

				var prop = (float)delta / _verticalMaximum;

				_scrollPosition.Y = delta;
				_verticalScrollbarThumb.Y = Bounds.Y + delta;

				origin.Y = (int)(prop*
				           (Widget.Bounds.Height - Bounds.Height +
				            (_horizontalScrollbarVisible ? _horizontalScrollbarThumb.Height : 0)));
			}

			_origin = origin;
			UpdateWidgetLocation();
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			_startBoundsPos = null;
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			var mousePosition = Desktop.MousePosition;
			if (mb == MouseButtons.Left)
			{
				if (_verticalScrollbarVisible && _verticalScrollbarThumb.Contains(mousePosition))
				{
					_startBoundsPos = mousePosition.Y - _verticalScrollbarThumb.Y;
					_scrollbarOrientation = Orientation.Vertical;
				}

				if (_horizontalScrollbarVisible && _horizontalScrollbarThumb.Contains(mousePosition))
				{
					_startBoundsPos = mousePosition.X - _horizontalScrollbarThumb.X;
					_scrollbarOrientation = Orientation.Horizontal;
				}
			}
		}

		public override void InternalRender(SpriteBatch batch)
		{
			if (Widget == null || !Widget.Visible)
			{
				return;
			}

			// Render child

			Widget.Render(batch);

			if (_horizontalScrollbarVisible)
			{
				HorizontalScrollBackground.Draw(batch, _horizontalScrollbarFrame);
				HorizontalScrollKnob.Draw(batch, _horizontalScrollbarThumb);
			}

			if (_verticalScrollbarVisible)
			{
				VerticalScrollBackground.Draw(batch, _verticalScrollbarFrame);
				VerticalScrollKnob.Draw(batch, _verticalScrollbarThumb);
			}
		}

		public void ApplyScrollPaneStyle(ScrollAreaStyle style)
		{
			HorizontalScrollBackground = style.HorizontalScrollBackground;
			HorizontalScrollKnob = style.HorizontalScrollKnob;
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;

			ApplyWidgetStyle(style);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var measureSize = Widget.Measure(availableSize);

			var horizontalScrollbarVisible = measureSize.X > availableSize.X;
			var verticalScrollbarVisible = measureSize.Y > availableSize.Y;
			if (horizontalScrollbarVisible || verticalScrollbarVisible)
			{
				if (horizontalScrollbarVisible)
				{
					measureSize.Y += HorizontalScrollKnob.Size.Y;
				}

				if (verticalScrollbarVisible)
				{
					measureSize.X += VerticalScrollKnob.Size.X;
				}
			}

			return measureSize;
		}

		public override void Arrange()
		{
			base.Arrange();

			if (Widget == null)
			{
				return;
			}

			var bounds = ClientBounds;
			var availableSize = bounds.Size;
			var measureSize = Widget.Measure(availableSize);

			_horizontalScrollbarVisible = measureSize.X > Bounds.Width;
			_verticalScrollbarVisible = measureSize.Y > Bounds.Height;
			if (_horizontalScrollbarVisible || _verticalScrollbarVisible)
			{
				if (_horizontalScrollbarVisible)
				{
					availableSize.Y -= HorizontalScrollKnob.Size.Y;

					if (availableSize.Y < 0)
					{
						availableSize.Y = 0;
					}
				}
				else
				{
					_origin.X = 0;
				}

				if (_verticalScrollbarVisible)
				{
					availableSize.X -= VerticalScrollKnob.Size.X;

					if (availableSize.X < 0)
					{
						availableSize.X = 0;
					}
				}
				else
				{
					_origin.Y = 0;
				}

				// Remeasure with scrollbars
				measureSize = Widget.Measure(availableSize);

				var bw = bounds.Width - (_verticalScrollbarVisible ? VerticalScrollBackground.Size.X : 0);

				_horizontalScrollbarFrame = new Rectangle(bounds.Left,
					bounds.Bottom - HorizontalScrollBackground.Size.Y,
					bw,
					HorizontalScrollBackground.Size.Y);

				var mw = measureSize.X;
				if (mw == 0)
				{
					mw = 1;
				}

				_horizontalScrollbarThumb = new Rectangle(bounds.Left + _scrollPosition.X,
					bounds.Bottom - HorizontalScrollBackground.Size.Y,
					Math.Max(HorizontalScrollKnob.Size.X, bw*bw/mw),
					HorizontalScrollKnob.Size.Y);

				var bh = bounds.Height - (_horizontalScrollbarVisible ? HorizontalScrollBackground.Size.Y : 0);

				_verticalScrollbarFrame = new Rectangle(
					bounds.Left + bounds.Width - VerticalScrollBackground.Size.X,
					bounds.Top,
					VerticalScrollBackground.Size.X,
					bh);

				var mh = measureSize.Y;
				if (mh == 0)
				{
					mh = 1;
				}

				_verticalScrollbarThumb = new Rectangle(
					bounds.Left + bounds.Width - VerticalScrollBackground.Size.X,
					bounds.Top + _scrollPosition.Y,
					VerticalScrollKnob.Size.X,
					Math.Max(VerticalScrollKnob.Size.Y, bh*bh/mh));

				_horizontalMaximum = bw - _horizontalScrollbarThumb.Width;
				_verticalMaximum = bh - _verticalScrollbarThumb.Height;

				if (_horizontalMaximum == 0)
				{
					_horizontalMaximum = 1;
				}

				if (_verticalMaximum == 0)
				{
					_verticalMaximum = 1;
				}

				bounds.Width = _horizontalScrollbarVisible ? measureSize.X : availableSize.X;
				bounds.Height = _verticalScrollbarVisible ? measureSize.Y : availableSize.Y;
			}

			Widget.LayoutChild(bounds);
			UpdateWidgetLocation();
		}
	}
}