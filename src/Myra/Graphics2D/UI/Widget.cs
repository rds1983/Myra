using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.Properties;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Core.Mathematics;
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class Widget : BaseObject
	{
		internal enum LayoutState
		{
			Normal,
			LocationInvalid,
			Invalid
		}
        #region PrivateData

        private int _left, _top;
		private int? _minWidth, _minHeight, _maxWidth, _maxHeight, _width, _height;
		private int _gridColumn, _gridRow, _gridColumnSpan = 1, _gridRowSpan = 1;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private LayoutState _layoutState = LayoutState.Invalid;
		private bool _isModal = false;
		private bool _measureDirty = true;
		private bool _active = false;
		private bool _isPlaced = false;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;
		private Point _lastLocationHint;

		private Rectangle _containerBounds;
		private Rectangle _bounds;
		private Rectangle _actualBounds;
		private bool _visible;

		private int _paddingLeft, _paddingRight, _paddingTop, _paddingBottom;
		private float _opacity = 1.0f;

		private bool _enabled;

        #endregion
        #region Data acsessor
        /// <summary>
        /// Internal use only. (MyraPad)
        /// </summary>
        [DefaultValue(Stylesheet.DefaultStyleName)]
		public string StyleName { get; set; }

		[Category("Layout")]
		[DefaultValue(0)]
		public int Left
		{
			get { return _left; }

			set
			{
				if (value == _left)
				{
					return;
				}

				_left = value;
				if (_layoutState == LayoutState.Normal)
				{
					_layoutState = LayoutState.LocationInvalid;
				}

				FireLocationChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int Top
		{
			get { return _top; }

			set
			{
				if (value == _top)
				{
					return;
				}

				_top = value;
				if (_layoutState == LayoutState.Normal)
				{
					_layoutState = LayoutState.LocationInvalid;
				}

				FireLocationChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? MinWidth
		{
			get { return _minWidth; }
			set
			{
				if (value == _minWidth)
				{
					return;
				}

				_minWidth = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? MaxWidth
		{
			get { return _maxWidth; }
			set
			{
				if (value == _maxWidth)
				{
					return;
				}

				_maxWidth = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? Width
		{
			get
			{
				return _width;
			}

			set
			{
				if (value == _width)
				{
					return;
				}

				_width = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? MinHeight
		{
			get { return _minHeight; }
			set
			{
				if (value == _minHeight)
				{
					return;
				}

				_minHeight = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? MaxHeight
		{
			get { return _maxHeight; }
			set
			{
				if (value == _maxHeight)
				{
					return;
				}

				_maxHeight = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(null)]
		public int? Height
		{
			get
			{
				return _height;
			}

			set
			{
				if (value == _height)
				{
					return;
				}

				_height = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int PaddingLeft
		{
			get { return _paddingLeft; }

			set
			{
				if (value == _paddingLeft)
				{
					return;
				}

				_paddingLeft = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int PaddingRight
		{
			get { return _paddingRight; }

			set
			{
				if (value == _paddingRight)
				{
					return;
				}

				_paddingRight = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int PaddingTop
		{
			get { return _paddingTop; }

			set
			{
				if (value == _paddingTop)
				{
					return;
				}

				_paddingTop = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int PaddingBottom
		{
			get { return _paddingBottom; }

			set
			{
				if (value == _paddingBottom)
				{
					return;
				}

				_paddingBottom = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(HorizontalAlignment.Left)]
		public virtual HorizontalAlignment HorizontalAlignment
		{
			get { return _horizontalAlignment; }

			set
			{
				if (value == _horizontalAlignment)
				{
					return;
				}

				_horizontalAlignment = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(VerticalAlignment.Top)]
		public virtual VerticalAlignment VerticalAlignment
		{
			get { return _verticalAlignment; }

			set
			{
				if (value == _verticalAlignment)
				{
					return;
				}

				_verticalAlignment = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int GridColumn
		{
			get { return _gridColumn; }

			set
			{
				if (value == _gridColumn)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridColumn = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(0)]
		public int GridRow
		{
			get { return _gridRow; }

			set
			{
				if (value == _gridRow)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridRow = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(1)]
		public int GridColumnSpan
		{
			get { return _gridColumnSpan; }

			set
			{
				if (value == _gridColumnSpan)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridColumnSpan = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DefaultValue(1)]
		public int GridRowSpan
		{
			get { return _gridRowSpan; }

			set
			{
				if (value == _gridRowSpan)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridRowSpan = value;
				InvalidateMeasure();
			}
		}

		[Category("Behavior")]
		[DefaultValue(true)]
		public virtual bool Enabled
		{
			get
			{
				return _enabled;
			}

			set
			{
				if (value == _enabled)
				{
					return;
				}

				_enabled = value;

				EnabledChanged.Invoke(this);
			}
		}

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool Visible
		{
			get { return _visible; }

			set
			{
				if (_visible == value)
				{
					return;
				}

				_visible = value;
				IsMouseInside = false;
				IsTouchInside = false;

				OnVisibleChanged();
			}
		}

		/// <summary>
		/// Determines whether a widget had been placed on Desktop
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public virtual bool IsPlaced
		{
			get
			{
				return _isPlaced;
			}

			internal set
			{
				_isPlaced = value;
				IsMouseInside = false;
				IsTouchInside = false;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public bool IsModal
		{
			get { return _isModal; }

			set
			{
				if (_isModal == value)
				{
					return;
				}

				_isModal = value;
				InvalidateMeasure();
			}
		}

		protected internal bool Active
		{
			get
			{
				return _active;
			}

			set
			{
				if (_active == value)
				{
					return;
				}

				_active = value;
				OnActiveChanged();
			}
		}

		[Category("Appearance")]
		[DefaultValue(1.0f)]
		public float Opacity
		{
			get
			{
				return _opacity;
			}

			set
			{
				if (value < 0 || value > 1.0f)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_opacity = value;
			}
		}
        #endregion

        #region Prop
        /// <summary>
        /// Dynamic layout expression
        /// </summary>
        public Layout2D Layout2d { get; set; } = Layout2D.NullLayout;

        [Category("Appearance")]
		public IBrush Background { get; set; }

		[Category("Appearance")]
		public IBrush OverBackground { get; set; }

		[Category("Appearance")]
		public IBrush DisabledBackground { get; set; }

		[Category("Appearance")]
		public IBrush FocusedBackground { get; set; }

		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool ClipToBounds { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool IsMouseInside { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool IsTouchInside { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public Container Parent { get; internal set; }

		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Rectangle Bounds
		{
			get
			{
				return _bounds;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ActualBounds
		{
			get
			{
				return _actualBounds;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ContainerBounds
		{
			get
			{
				return _containerBounds;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int PaddingWidth
		{
			get { return _paddingLeft + _paddingRight; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public int PaddingHeight
		{
			get { return _paddingTop + _paddingBottom; }
		}

		[Browsable(false)]
		[XmlIgnore]
		internal protected virtual bool AcceptsKeyboardFocus
		{
			get { return false; }
		}

		[Browsable(false)]
		[XmlIgnore]
		internal protected virtual bool AcceptsMouseWheelFocus
		{
			get { return false; }
		}

		[Browsable(false)]
		[XmlIgnore]
		internal protected virtual bool MouseWheelFocusCanBeNull
		{
			get { return true; }
		}

		/// <summary>
		/// When Width/Height is set and HorizontalAlignment/VerticalAlignment is set to Stretch
		/// This property determines what to use for layout
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal protected virtual bool PrioritizeStrethOverSize
		{
			get { return true; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsKeyboardFocused
		{
			get
			{
				return Desktop.FocusedKeyboardWidget == this;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsMouseWheelFocused
		{
			get
			{
				return Desktop.FocusedMouseWheelWidget == this;
			}
		}

		protected virtual bool UseHoverRenderable
		{
			get
			{
				return IsMouseInside && Active;
			}
		}
        #endregion
        #region Events
        public event EventHandler VisibleChanged;
		public event EventHandler MeasureChanged;
		public event EventHandler EnabledChanged;

		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;
		public event EventHandler LayoutUpdated;

		public event EventHandler MouseLeft;
		public event EventHandler MouseEntered;
		public event EventHandler MouseMoved;

		public event EventHandler TouchLeft;
		public event EventHandler TouchEntered;
		public event EventHandler TouchMoved;
		public event EventHandler TouchDown;
		public event EventHandler TouchUp;
		public event EventHandler TouchDoubleClick;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;
		public event EventHandler<GenericEventArgs<char>> Char;
        #endregion

        public Widget()
		{
			Visible = true;
			Enabled = true;
		}

        #region Functions
        public virtual IBrush GetCurrentBackground()
		{
			var result = Background;

			if (!Enabled && DisabledBackground != null)
			{
				result = DisabledBackground;
			}
			else if (Enabled && IsKeyboardFocused && FocusedBackground != null)
			{
				result = FocusedBackground;
			}
			else if (UseHoverRenderable && OverBackground != null)
			{
				result = OverBackground;
			}

			return result;
		}

		public void Render(RenderContext context)
		{
			if (!Visible)
			{
				return;
			}

			UpdateLayout();

			var view = Rectangle.Intersect(context.View, Bounds);
			if (view.Width == 0 || view.Height == 0)
			{
				return;
			}

			var batch = context.Batch;
			var oldScissorRectangle = CrossEngineStuff.GetScissor();
			if (ClipToBounds && !MyraEnvironment.DisableClipping)
			{
				var newScissorRectangle = Rectangle.Intersect(oldScissorRectangle, view);

				if (newScissorRectangle.IsEmpty)
				{
					return;
				}

				context.Flush();

				CrossEngineStuff.SetScissor(newScissorRectangle);
			}

			var oldOpacity = context.Opacity;

			context.Opacity *= Opacity;

			// Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				context.Draw(background, Bounds);
			}

			var oldView = context.View;
			context.View = view;
			InternalRender(context);
			context.View = oldView;

			if (MyraEnvironment.DrawWidgetsFrames)
			{
				batch.DrawRectangle(Bounds, Color.LightGreen);
			}

			if (MyraEnvironment.DrawKeyboardFocusedWidgetFrame && IsKeyboardFocused)
			{
				batch.DrawRectangle(Bounds, Color.Red);
			}

			if (MyraEnvironment.DrawMouseWheelFocusedWidgetFrame && IsMouseWheelFocused)
			{
				batch.DrawRectangle(Bounds, Color.Yellow);
			}

			if (ClipToBounds && !MyraEnvironment.DisableClipping)
			{
				context.Flush();
				CrossEngineStuff.SetScissor(oldScissorRectangle);
			}

			context.Opacity = oldOpacity;
		}

		public virtual void InternalRender(RenderContext context)
		{
		}

		public Point Measure(Point availableSize)
		{
			if (!_measureDirty && _lastMeasureAvailableSize == availableSize)
			{
				return _lastMeasureSize;
			}

			Point result;

			if (Width.HasValue && Height.HasValue)
			{
				result = new Point(Width.Value, Height.Value);
			}
			else
			{
				// Lerp available size by Width/Height or MaxWidth/MaxHeight
				if (!PrioritizeStrethOverSize || HorizontalAlignment != HorizontalAlignment.Stretch)
				{
					if (Width != null && availableSize.X > Width.Value)
					{
						availableSize.X = Width.Value;
					}
					else if (MaxWidth != null && availableSize.X > MaxWidth.Value)
					{
						availableSize.X = MaxWidth.Value;
					}
				}

				if (!PrioritizeStrethOverSize || VerticalAlignment != VerticalAlignment.Stretch)
				{
					if (Height != null && availableSize.Y > Height.Value)
					{
						availableSize.Y = Height.Value;
					}
					else if (MaxHeight != null && availableSize.Y > MaxHeight.Value)
					{
						availableSize.Y = MaxHeight.Value;
					}
				}

				// Do the actual measure
				result = InternalMeasure(availableSize);

				// Result lerp
				if (!PrioritizeStrethOverSize || HorizontalAlignment != HorizontalAlignment.Stretch)
				{
					if (Width.HasValue)
					{
						result.X = Width.Value;
					}
					else
					{
						if (MinWidth.HasValue && result.X < MinWidth.Value)
						{
							result.X = MinWidth.Value;
						}

						if (MaxWidth.HasValue && result.X > MaxWidth.Value)
						{
							result.X = MaxWidth.Value;
						}
					}
				}

				if (!PrioritizeStrethOverSize || VerticalAlignment != VerticalAlignment.Stretch)
				{
					if (Height.HasValue)
					{
						result.Y = Height.Value;
					}
					else
					{
						if (MinHeight.HasValue && result.Y < MinHeight.Value)
						{
							result.Y = MinHeight.Value;
						}

						if (MaxHeight.HasValue && result.Y > MaxHeight.Value)
						{
							result.Y = MaxHeight.Value;
						}
					}
				}
			}

			result.X += PaddingWidth;
			result.Y += PaddingHeight;

			_lastMeasureSize = result;
			_lastMeasureAvailableSize = availableSize;
			_measureDirty = false;

			return result;
		}

		protected virtual Point InternalMeasure(Point availableSize)
		{
			return Point.Zero;
		}

		public virtual void Arrange()
		{
		}

		public void Layout(Rectangle containerBounds)
		{
			if (_containerBounds != containerBounds)
			{
				InvalidateLayout();
				_containerBounds = containerBounds;
			}

			UpdateLayout();
		}

		public void InvalidateLayout()
		{
			_layoutState = LayoutState.Invalid;
		}

		internal virtual void MoveChildren(Point delta)
		{
			_bounds.X += delta.X;
			_bounds.Y += delta.Y;

			_actualBounds.X += delta.X;
			_actualBounds.Y += delta.Y;

			_containerBounds.X += delta.X;
			_containerBounds.Y += delta.Y;
		}

		public virtual void UpdateLayout()
		{
			if (_layoutState == LayoutState.Normal)
			{
				return;
			}

			if (_layoutState == LayoutState.Invalid)
			{
				// Full rearrange
				Point size;
				if (HorizontalAlignment != HorizontalAlignment.Stretch ||
					VerticalAlignment != VerticalAlignment.Stretch)
				{
					size = Measure(_containerBounds.Size());
				}
				else
				{
					size = _containerBounds.Size();
				}

				if (size.X > _containerBounds.Width)
				{
					size.X = _containerBounds.Width;
				}

				if (size.Y > _containerBounds.Height)
				{
					size.Y = _containerBounds.Height;
				}

				// Resolve possible conflict beetween Alignment set to Streth and Size explicitly set
				var containerSize = _containerBounds.Size();

				if (!PrioritizeStrethOverSize)
				{
					if (HorizontalAlignment == HorizontalAlignment.Stretch && Width != null && Width.Value < containerSize.X)
					{
						containerSize.X = Width.Value;
					}

					if (VerticalAlignment == VerticalAlignment.Stretch && Height != null && Height.Value < containerSize.Y)
					{
						containerSize.Y = Height.Value;
					}
				}

				// Align
				var controlBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment);
				controlBounds.Offset(_containerBounds.Location);

				controlBounds.Offset(Left, Top);

				_bounds = controlBounds;
				_actualBounds = CalculateClientBounds(controlBounds);

				Arrange();
			}
			else
			{
				// Only location
				MoveChildren(new Point(Left - _lastLocationHint.X, Top - _lastLocationHint.Y));
			}

			_lastLocationHint = new Point(Left, Top);
			_layoutState = LayoutState.Normal;

			HandleMouseMovement();

			LayoutUpdated.Invoke(this);
		}

		private Widget FindWidgetBy(Func<Widget, bool> finder)
		{
			if (finder(this))
			{
				return this;
			}

			var asContainer = this as Container;
			if (asContainer != null)
			{
				foreach (var widget in asContainer.ChildrenCopy)
				{
					var result = widget.FindWidgetBy(finder);
					if (result != null)
					{
						return result;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Finds a widget by id
		/// Returns null if not found
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Widget FindWidgetById(string id)
		{
			return FindWidgetBy(w => w.Id == id);
		}

		/// <summary>
		/// Find a widget by id
		/// Throws exception if not found
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Widget EnsureWidgetById(string id)
		{
			var result = FindWidgetById(id);
			if (result == null)
			{
                throw new Exception(string.Format($"Could not find widget with id {id}"));
			}

			return result;
		}

		public virtual void InvalidateMeasure()
		{
			_measureDirty = true;

			InvalidateLayout();

			MeasureChanged.Invoke(this);
		}

		public void ApplyWidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;
			MinWidth = style.MinWidth;
			MinHeight = style.MinHeight;
			MaxWidth = style.MaxWidth;
			MaxHeight = style.MaxHeight;

			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;

			if (style.PaddingLeft != null)
			{
				PaddingLeft = style.PaddingLeft.Value;
			}
			if (style.PaddingRight != null)
			{
				PaddingRight = style.PaddingRight.Value;
			}
			if (style.PaddingTop != null)
			{
				PaddingTop = style.PaddingTop.Value;
			}
			if (style.PaddingBottom != null)
			{
				PaddingBottom = style.PaddingBottom.Value;
			}
		}

		public void SetStyle(Stylesheet stylesheet, string name)
		{
			StyleName = name;

			if (StyleName != null)
			{
				InternalSetStyle(stylesheet, StyleName);
			}
		}

		public void SetStyle(string name)
		{
			SetStyle(Stylesheet.Current, name);
		}

		protected virtual void InternalSetStyle(Stylesheet stylesheet, string name)
		{
		}

		public virtual void OnMouseLeft()
		{
			IsMouseInside = false;
			MouseLeft.Invoke(this);
		}

		public virtual void OnMouseEntered()
		{
			IsMouseInside = true;
			MouseEntered.Invoke(this);
		}

		public virtual void OnMouseMoved()
		{
			IsMouseInside = true;
			MouseMoved.Invoke(this);
		}

		public virtual void OnTouchDoubleClick()
		{
			TouchDoubleClick.Invoke(this);
		}

		public virtual void OnMouseWheel(float delta)
		{
			MouseWheelChanged.Invoke(this, delta);
		}

		public virtual void OnTouchLeft()
		{
			IsTouchInside = false;
			TouchLeft.Invoke(this);
		}

		public virtual void OnTouchEntered()
		{
			IsTouchInside = true;
			TouchEntered.Invoke(this);
		}

		public virtual void OnTouchMoved()
		{
			IsTouchInside = true;
			TouchMoved.Invoke(this);
		}

		public virtual void OnTouchDown()
		{
			IsTouchInside = true;

			if (Enabled && AcceptsKeyboardFocus)
			{
				Desktop.FocusedKeyboardWidget = this;
			}

			if (Enabled && AcceptsMouseWheelFocus)
			{
				Desktop.FocusedMouseWheelWidget = this;
			}

			TouchDown.Invoke(this);
		}

		public virtual void OnTouchUp()
		{
			IsTouchInside = false;
			TouchUp.Invoke(this);
		}

		protected void FireKeyDown(Keys k)
		{
			KeyDown.Invoke(this, k);
		}

		public virtual void OnKeyDown(Keys k)
		{
			FireKeyDown(k);
		}

		public virtual void OnKeyUp(Keys k)
		{
			KeyUp.Invoke(this, k);
		}

		public virtual void OnChar(char c)
		{
			Char.Invoke(this, c);
		}

		public virtual void OnVisibleChanged()
		{
			InvalidateMeasure();
			VisibleChanged.Invoke(this);
		}

		public virtual void OnLostKeyboardFocus()
		{
		}

		public virtual void OnGotKeyboardFocus()
		{
		}

		protected internal virtual void OnActiveChanged()
		{
		}

		internal Rectangle CalculateClientBounds(Rectangle clientBounds)
		{
			clientBounds.X += _paddingLeft;
			clientBounds.Y += _paddingTop;

			clientBounds.Width -= PaddingWidth;
			if (clientBounds.Width < 0)
			{
				clientBounds.Width = 0;
			}

			clientBounds.Height -= PaddingHeight;
			if (clientBounds.Height < 0)
			{
				clientBounds.Height = 0;
			}

			return clientBounds;
		}

		public void RemoveFromParent()
		{
			if (Parent == null)
			{
				return;
			}

			Parent.RemoveChild(this);
		}

		public void RemoveFromDesktop()
		{
			Desktop.Widgets.Remove(this);
		}

		private void FireLocationChanged()
		{
			LocationChanged.Invoke(this);
		}

		private void FireSizeChanged()
		{
			SizeChanged.Invoke(this);
		}

		internal void HandleTouchDoubleClick()
		{
			if (!Visible)
			{
				return;
			}

			var isTouchOver = Bounds.Contains(Desktop.TouchPosition);
			if (isTouchOver)
			{
				OnTouchDoubleClick();
			}
		}

		internal bool HandleMouseMovement()
		{
			var isMouseOver = Bounds.Contains(Desktop.MousePosition);
			var wasMouseOver = IsMouseInside;

			if (isMouseOver && !wasMouseOver)
			{
				OnMouseEntered();
			}

			if (!isMouseOver && wasMouseOver)
			{
				OnMouseLeft();
			}

			if (isMouseOver && wasMouseOver)
			{
				OnMouseMoved();
			}

			return isMouseOver;
		}

		internal void HandleTouchDown()
		{
			if (!Visible)
			{
				return;
			}

			if (Bounds.Contains(Desktop.TouchPosition))
			{
				OnTouchDown();
			}
		}

		internal void HandleTouchUp()
		{
			if (!Visible)
			{
				return;
			}

			if (IsTouchInside)
			{
				OnTouchUp();
			}
		}

		internal bool HandleTouchMovement()
		{
			var isTouchOver = Bounds.Contains(Desktop.TouchPosition);
			var wasTouchOver = IsTouchInside;

			if (isTouchOver && !wasTouchOver)
			{
				OnTouchEntered();
			}

			if (!isTouchOver && wasTouchOver)
			{
				OnTouchLeft();
			}

			if (isTouchOver && wasTouchOver)
			{
				OnTouchMoved();
			}

			return isTouchOver;
		}
	}
    #endregion
}