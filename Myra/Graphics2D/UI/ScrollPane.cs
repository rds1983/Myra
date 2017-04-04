using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

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

		[HiddenInEditor]
		[JsonIgnore]
		public Point ScrollPosition
		{
			get { return _scrollPosition; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable HorizontalScrollBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable HorizontalScrollKnob { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable VerticalScrollBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable VerticalScrollKnob { get; set; }

		/// <summary>
		/// Same as Widget. Used by the JSON serializer.
		/// </summary>
		[HiddenInEditor]
		public T Child
		{
			get { return Widget; }
			set { Widget = value; }
		}

		public ScrollPane(ScrollAreaStyle style)
		{
			ClipToBounds = true;
			CanFocus = true;
			_horizontalScrollbarVisible = _verticalScrollbarVisible = false;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			if (style != null)
			{
				ApplyScrollPaneStyle(style);
			}
		}

		public ScrollPane() : this(DefaultAssets.UIStylesheet.ScrollAreaStyle)
		{
		}

		private void UpdateWidgetLocation()
		{
			if (Widget == null) return;

			Widget.XHint = -_origin.X;
			Widget.YHint = -_origin.Y;
		}

		private void MoveThumb(int delta)
		{
			var origin = _origin;

			var bounds = LayoutBounds;
			if (_scrollbarOrientation == Orientation.Horizontal)
			{
				var newPos = delta + _scrollPosition.X;
				if (newPos < 0)
				{
					newPos = 0;
				}

				if (newPos > _horizontalMaximum)
				{
					newPos = _horizontalMaximum;
				}

				var prop = (float)newPos / _horizontalMaximum;

				_scrollPosition.X = newPos;
				_horizontalScrollbarThumb.X = LayoutBounds.X + newPos;

				origin.X = (int)(prop *
						   (Widget.Bounds.Width - bounds.Width + (_verticalScrollbarVisible ? _verticalScrollbarThumb.Width : 0)));
			}
			else
			{
				var newPos = delta + _scrollPosition.Y;

				if (newPos < 0)
				{
					newPos = 0;
				}

				if (newPos > _verticalMaximum)
				{
					newPos = _verticalMaximum;
				}

				var prop = (float)newPos / _verticalMaximum;

				_scrollPosition.Y = newPos;
				_verticalScrollbarThumb.Y = LayoutBounds.Y + newPos;

				origin.Y = (int)(prop *
						   (Widget.Bounds.Height - bounds.Height +
							(_horizontalScrollbarVisible ? _horizontalScrollbarThumb.Height : 0)));
			}

			_origin = origin;
			UpdateWidgetLocation();
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (!_startBoundsPos.HasValue) return;

			int delta;
			if (_scrollbarOrientation == Orientation.Horizontal)
			{
				delta = position.X - AbsoluteBounds.X - _startBoundsPos.Value - _scrollPosition.X;
			}
			else
			{
				delta = position.Y - AbsoluteBounds.Y - _startBoundsPos.Value - _scrollPosition.Y;
			}

			MoveThumb(delta);
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

			if (mb != MouseButtons.Left) return;

			var bounds = RenderBounds;
			if (_verticalScrollbarVisible && _verticalScrollbarThumb.Add(bounds.Location).Contains(mousePosition))
			{
				_startBoundsPos = mousePosition.Y - _verticalScrollbarThumb.Y - bounds.Y;
				_scrollbarOrientation = Orientation.Vertical;
			}

			if (_horizontalScrollbarVisible && _horizontalScrollbarThumb.Add(bounds.Location).Contains(mousePosition))
			{
				_startBoundsPos = mousePosition.X - _horizontalScrollbarThumb.X - bounds.X;
				_scrollbarOrientation = Orientation.Horizontal;
			}
		}

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (!_verticalScrollbarVisible)
			{
				return;
			}

			if (delta < 0)
			{
				_scrollbarOrientation = Orientation.Vertical;
				MoveThumb(10);
			}
			else if (delta > 0)
			{
				_scrollbarOrientation = Orientation.Vertical;
				MoveThumb(-10);
			}
		}

		public override void InternalRender(SpriteBatch batch)
		{
			if (Widget == null || !Widget.Visible)
			{
				return;
			}

			// Render child
			base.InternalRender(batch);

			var bounds = RenderBounds;

			if (_horizontalScrollbarVisible)
			{
				HorizontalScrollBackground.Draw(batch, _horizontalScrollbarFrame.Add(bounds.Location));
				HorizontalScrollKnob.Draw(batch, _horizontalScrollbarThumb.Add(bounds.Location));
			}

			if (_verticalScrollbarVisible)
			{
				VerticalScrollBackground.Draw(batch, _verticalScrollbarFrame.Add(bounds.Location));
				VerticalScrollKnob.Draw(batch, _verticalScrollbarThumb.Add(bounds.Location));
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
			if (Widget == null)
			{
				return Point.Zero;
			}

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

			var bounds = LayoutBounds;
			var availableSize = bounds.Size();
			var measureSize = Widget.Measure(availableSize);

			_horizontalScrollbarVisible = measureSize.X > bounds.Width;
			_verticalScrollbarVisible = measureSize.Y > bounds.Height;
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

			Widget.Layout(bounds);
			UpdateWidgetLocation();
		}

		public void ResetScroll()
		{
			_origin = Point.Zero;
			UpdateWidgetLocation();
		}
	}
}