using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ScrollPane : SingleItemContainer<Widget>
	{
		private Orientation _scrollbarOrientation;
		private bool _horizontalScrollbarVisible, _verticalScrollbarVisible;
		private Rectangle _horizontalScrollbarFrame, _horizontalScrollbarThumb;
		private Rectangle _verticalScrollbarFrame, _verticalScrollbarThumb;
		private Point _scrollPosition;
		private int? _startBoundsPos;
		private int _horizontalMaximum, _verticalMaximum;

		[HiddenInEditor]
		[JsonIgnore]
		public Point ScrollPosition
		{
			get { return _scrollPosition; }
			set
			{
				_scrollPosition = value;
				UpdateWidgetLocation();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Point ScrollMaximum
		{
			get { return new Point(_horizontalMaximum, _verticalMaximum); }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion HorizontalScrollBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion HorizontalScrollKnob { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion VerticalScrollBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion VerticalScrollKnob { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public override Widget Widget
		{
			get { return base.Widget; }
			set
			{
				base.Widget = value;
				ResetScroll();
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public bool AllowHorizontalScrolling { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public bool AllowVerticalScrolling { get; set; }

		/// <summary>
		/// Same as Widget. Used by the JSON serializer.
		/// </summary>
		[HiddenInEditor]
		public Widget Child
		{
			get { return Widget; }
			set { Widget = value; }
		}

		public ScrollPane(ScrollPaneStyle style)
		{
			ClipToBounds = true;
			CanFocus = true;
			_horizontalScrollbarVisible = _verticalScrollbarVisible = false;

			AllowVerticalScrolling = AllowHorizontalScrolling = true;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			if (style != null)
			{
				ApplyScrollPaneStyle(style);
			}
		}

		public ScrollPane(string style)
			: this(Stylesheet.Current.ScrollPaneStyles[style])
		{
		}

		public ScrollPane() : this(Stylesheet.Current.ScrollPaneStyle)
		{
		}

		private void UpdateWidgetLocation()
		{
			if (Widget == null) return;

			var prop = Vector2.Zero;

			if (_horizontalMaximum > 0)
			{
				prop.X = (float)_scrollPosition.X / _horizontalMaximum;
			}

			if (_verticalMaximum != 0)
			{
				prop.Y = (float)_scrollPosition.Y / _verticalMaximum;
			}

			var bounds = ActualBounds;
			var origin = new Point((int)(prop.X *
										  (Widget.Bounds.Width - bounds.Width +
										   (_verticalScrollbarVisible ? _verticalScrollbarThumb.Width : 0))),
				(int)(prop.Y *
					   (Widget.Bounds.Height - bounds.Height +
						(_horizontalScrollbarVisible ? _horizontalScrollbarThumb.Height : 0)))
			);

			Widget.XHint = -origin.X;
			Widget.YHint = -origin.Y;
		}

		private void MoveThumb(int delta)
		{
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

				_scrollPosition.X = newPos;
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

				_scrollPosition.Y = newPos;
			}

			UpdateWidgetLocation();
		}

		internal override void MoveChildren(Point delta)
		{
			base.MoveChildren(delta);

			if (_horizontalScrollbarVisible)
			{
				_horizontalScrollbarFrame.Offset(delta.X, delta.Y);
				_horizontalScrollbarThumb.Offset(delta.X, delta.Y);
			}

			if (_verticalScrollbarVisible)
			{
				_verticalScrollbarFrame.Offset(delta.X, delta.Y);
				_verticalScrollbarThumb.Offset(delta.X, delta.Y);
			}
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

			var r = _verticalScrollbarThumb;
			r.Y += _scrollPosition.Y;
			if (_verticalScrollbarVisible && r.Contains(mousePosition))
			{
				_startBoundsPos = mousePosition.Y;
				_scrollbarOrientation = Orientation.Vertical;
			}

			r = _horizontalScrollbarThumb;
			r.X += _scrollPosition.X;
			if (_horizontalScrollbarVisible && r.Contains(mousePosition))
			{
				_startBoundsPos = mousePosition.X;
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

		public override void InternalRender(RenderContext context)
		{
			if (Widget == null || !Widget.Visible)
			{
				return;
			}

			// Render child
			base.InternalRender(context);

			if (_horizontalScrollbarVisible)
			{
				context.Draw(HorizontalScrollBackground, _horizontalScrollbarFrame);

				var r = _horizontalScrollbarThumb;
				r.X += _scrollPosition.X;
				context.Draw(HorizontalScrollKnob, r);
			}

			if (_verticalScrollbarVisible)
			{
				context.Draw(VerticalScrollBackground, _verticalScrollbarFrame);

				var r = _verticalScrollbarThumb;
				r.Y += _scrollPosition.Y;
				context.Draw(VerticalScrollKnob, r);
			}
		}

		public void ApplyScrollPaneStyle(ScrollPaneStyle style)
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

			var horizontalScrollbarVisible = AllowHorizontalScrolling && measureSize.X > availableSize.X;
			var verticalScrollbarVisible = AllowVerticalScrolling && measureSize.Y > availableSize.Y;
			if (horizontalScrollbarVisible || verticalScrollbarVisible)
			{
				if (horizontalScrollbarVisible)
				{
					measureSize.Y += HorizontalScrollKnob.Bounds.Width;
				}

				if (verticalScrollbarVisible)
				{
					measureSize.X += VerticalScrollKnob.Bounds.Height;
				}
			}

			return measureSize;
		}

		public override void Arrange()
		{
			if (Widget == null)
			{
				return;
			}

			var bounds = ActualBounds;
			var availableSize = bounds.Size();
			var measureSize = Widget.Measure(availableSize);

			_horizontalScrollbarVisible = AllowHorizontalScrolling && measureSize.X > bounds.Width;
			_verticalScrollbarVisible = AllowVerticalScrolling && measureSize.Y > bounds.Height;
			if (_horizontalScrollbarVisible || _verticalScrollbarVisible)
			{
				if (_horizontalScrollbarVisible)
				{
					availableSize.Y -= HorizontalScrollKnob.Bounds.Height;

					if (availableSize.Y < 0)
					{
						availableSize.Y = 0;
					}
				}

				if (_verticalScrollbarVisible)
				{
					availableSize.X -= VerticalScrollKnob.Bounds.Width;

					if (availableSize.X < 0)
					{
						availableSize.X = 0;
					}
				}

				// Remeasure with scrollbars
				measureSize = Widget.Measure(availableSize);

				var bw = bounds.Width - (_verticalScrollbarVisible ? VerticalScrollBackground.Bounds.Width : 0);

				_horizontalScrollbarFrame = new Rectangle(bounds.Left,
					bounds.Bottom - HorizontalScrollBackground.Bounds.Height,
					bw,
					HorizontalScrollBackground.Bounds.Height);

				var mw = measureSize.X;
				if (mw == 0)
				{
					mw = 1;
				}

				_horizontalScrollbarThumb = new Rectangle(bounds.Left,
					bounds.Bottom - HorizontalScrollBackground.Bounds.Height,
					Math.Max(HorizontalScrollKnob.Bounds.Width, bw * bw / mw),
					HorizontalScrollKnob.Bounds.Height);

				var bh = bounds.Height - (_horizontalScrollbarVisible ? HorizontalScrollBackground.Bounds.Height : 0);

				_verticalScrollbarFrame = new Rectangle(
					bounds.Left + bounds.Width - VerticalScrollBackground.Bounds.Width,
					bounds.Top,
					VerticalScrollBackground.Bounds.Width,
					bh);

				var mh = measureSize.Y;
				if (mh == 0)
				{
					mh = 1;
				}

				_verticalScrollbarThumb = new Rectangle(
					bounds.Left + bounds.Width - VerticalScrollBackground.Bounds.Width,
					bounds.Top,
					VerticalScrollKnob.Bounds.Width,
					Math.Max(VerticalScrollKnob.Bounds.Height, bh * bh / mh));

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

				if (_scrollPosition.X < 0)
				{
					_scrollPosition.X = 0;
				}

				if (_scrollPosition.X > _horizontalMaximum)
				{
					_scrollPosition.X = _horizontalMaximum;
				}

				if (_scrollPosition.Y < 0)
				{
					_scrollPosition.Y = 0;
				}

				if (_scrollPosition.Y > _verticalMaximum)
				{
					_scrollPosition.Y = _verticalMaximum;
				}
			}

			Widget.Layout(bounds);

			UpdateWidgetLocation();
		}

		public void ResetScroll()
		{
			_scrollPosition = Point.Zero;
			UpdateWidgetLocation();
		}

		public override void OnDesktopChanging()
		{
			base.OnDesktopChanging();

			if (Desktop != null)
			{
				Desktop.MouseMoved -= DesktopMouseMoved;
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			if (Desktop != null)
			{
				Desktop.MouseMoved += DesktopMouseMoved;
			}
		}

		private void DesktopMouseMoved(object sender, GenericEventArgs<Point> e)
		{
			if (!_startBoundsPos.HasValue) return;

			var position = e.Data;

			int delta;
			if (_scrollbarOrientation == Orientation.Horizontal)
			{
				delta = position.X - _startBoundsPos.Value;
				_startBoundsPos = position.X;
			}
			else
			{
				delta = position.Y - _startBoundsPos.Value;
				_startBoundsPos = position.Y;
			}

			MoveThumb(delta);
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyScrollPaneStyle(stylesheet.ScrollPaneStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ScrollPaneStyles.Keys.ToArray();
		}
	}
}