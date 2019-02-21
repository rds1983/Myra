using System;
using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ScrollPane : SingleItemContainer<Widget>, IContent
	{
		private Orientation _scrollbarOrientation;
		internal bool _horizontalScrollbarVisible, _verticalScrollbarVisible;
		internal Rectangle _horizontalScrollbarFrame, _horizontalScrollbarThumb;
		internal Rectangle _verticalScrollbarFrame, _verticalScrollbarThumb;
		private Point _scrollPosition;
		private int? _startBoundsPos;
		private int _horizontalMaximum, _verticalMaximum;

		[HiddenInEditor]
		[XmlIgnore]
		internal Point ScrollMaximumPixels
		{
			get
			{
				var bounds = ActualBounds;

				return new Point(InternalChild.Bounds.Width - bounds.Width + (_verticalScrollbarVisible ? _verticalScrollbarThumb.Width : 0),
								 InternalChild.Bounds.Height - bounds.Height + (_horizontalScrollbarVisible ? _horizontalScrollbarThumb.Height : 0));
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
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
		[XmlIgnore]
		public Point ScrollMaximum
		{
			get { return new Point(_horizontalMaximum, _verticalMaximum); }
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable HorizontalScrollBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable HorizontalScrollKnob { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable VerticalScrollBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable VerticalScrollKnob { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Widget Content
		{
			get
			{
				return base.InternalChild;
			}
			set
			{
				base.InternalChild = value;
				ResetScroll();
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		[Obsolete("Use Content")]
		public Widget Widget
		{
			get
			{
				return Content;
			}
			set
			{
				Content = value;
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public bool AllowHorizontalScrolling { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public bool AllowVerticalScrolling { get; set; }

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		[DefaultValue(true)]
		public override bool ClipToBounds
		{
			get { return base.ClipToBounds; }
			set { base.ClipToBounds = value; }
		}

		[DefaultValue(true)]
		public override bool CanFocus
		{
			get { return base.CanFocus; }
			set { base.CanFocus = value; }
		}

		/// <summary>
		/// Same as Widget. Used by the JSON serializer.
		/// </summary>
		[HiddenInEditor]
		public Widget Child
		{
			get { return InternalChild; }
			set { InternalChild = value; }
		}

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}
			set
			{
				if (Desktop != null)
				{
					Desktop.MouseMoved -= DesktopMouseMoved;
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.MouseMoved += DesktopMouseMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
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
			if (InternalChild == null) return;

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
			var maximumPixels = ScrollMaximumPixels;
			var origin = new Point((int)(prop.X * maximumPixels.X), (int)(prop.Y * maximumPixels.Y));

			InternalChild.Left = -origin.X;
			InternalChild.Top = -origin.Y;
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

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_startBoundsPos = null;
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			var mousePosition = Desktop.MousePosition;

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
			if (InternalChild == null || !InternalChild.Visible)
			{
				return;
			}

			// Render child
			base.InternalRender(context);

			if (_horizontalScrollbarVisible)
			{
				if (HorizontalScrollBackground != null)
				{
					context.Draw(HorizontalScrollBackground, _horizontalScrollbarFrame);
				}

				var r = _horizontalScrollbarThumb;
				r.X += _scrollPosition.X;
				context.Draw(HorizontalScrollKnob, r);
			}

			if (_verticalScrollbarVisible)
			{
				if (VerticalScrollBackground != null)
				{
					context.Draw(VerticalScrollBackground, _verticalScrollbarFrame);
				}

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
			if (InternalChild == null)
			{
				return Point.Zero;
			}

			var measureSize = InternalChild.Measure(availableSize);

			var horizontalScrollbarVisible = AllowHorizontalScrolling && measureSize.X > availableSize.X;
			var verticalScrollbarVisible = AllowVerticalScrolling && measureSize.Y > availableSize.Y;
			if (horizontalScrollbarVisible || verticalScrollbarVisible)
			{
				if (horizontalScrollbarVisible)
				{
					measureSize.Y += HorizontalScrollKnob.Size.X;
				}

				if (verticalScrollbarVisible)
				{
					measureSize.X += VerticalScrollKnob.Size.Y;
				}
			}

			return measureSize;
		}

		public override void Arrange()
		{
			if (InternalChild == null)
			{
				return;
			}

			var bounds = ActualBounds;
			var availableSize = bounds.Size();
			var measureSize = InternalChild.Measure(availableSize);

			_horizontalScrollbarVisible = AllowHorizontalScrolling && measureSize.X > bounds.Width;
			_verticalScrollbarVisible = AllowVerticalScrolling && measureSize.Y > bounds.Height;
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

				if (_verticalScrollbarVisible)
				{
					availableSize.X -= VerticalScrollKnob.Size.X;

					if (availableSize.X < 0)
					{
						availableSize.X = 0;
					}
				}

				// Remeasure with scrollbars
				measureSize = InternalChild.Measure(availableSize);

				var vsWidth = VerticalScrollBackground != null ? VerticalScrollBackground.Size.X : VerticalScrollKnob.Size.X;
				var hsHeight = HorizontalScrollBackground != null ? HorizontalScrollBackground.Size.Y : HorizontalScrollKnob.Size.Y;

				var bw = bounds.Width - (_verticalScrollbarVisible ? vsWidth : 0);

				_horizontalScrollbarFrame = new Rectangle(bounds.Left,
					bounds.Bottom - hsHeight,
					bw,
					hsHeight);

				var mw = measureSize.X;
				if (mw == 0)
				{
					mw = 1;
				}

				_horizontalScrollbarThumb = new Rectangle(bounds.Left,
					bounds.Bottom - hsHeight,
					Math.Max(HorizontalScrollKnob.Size.X, bw * bw / mw),
					HorizontalScrollKnob.Size.Y);

				var bh = bounds.Height - (_horizontalScrollbarVisible ? hsHeight : 0);

				_verticalScrollbarFrame = new Rectangle(
					bounds.Left + bounds.Width - vsWidth,
					bounds.Top,
					vsWidth,
					bh);

				var mh = measureSize.Y;
				if (mh == 0)
				{
					mh = 1;
				}

				_verticalScrollbarThumb = new Rectangle(
					bounds.Left + bounds.Width - vsWidth,
					bounds.Top,
					VerticalScrollKnob.Size.X,
					Math.Max(VerticalScrollKnob.Size.Y, bh * bh / mh));

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

			InternalChild.Layout(bounds);

			UpdateWidgetLocation();
		}

		public void ResetScroll()
		{
			_scrollPosition = Point.Zero;
			UpdateWidgetLocation();
		}

		private void DesktopMouseMoved(object sender, EventArgs args)
		{
			if (!_startBoundsPos.HasValue) return;

			var position = Desktop.MousePosition;

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

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			_startBoundsPos = null;
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