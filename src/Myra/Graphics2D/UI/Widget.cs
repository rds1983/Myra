using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Core.Mathematics;
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class Widget : IItemWithId
	{
		internal enum LayoutState
		{
			Normal,
			LocationInvalid,
			Invalid
		}

		private int _left, _top;
		private int? _minWidth, _minHeight, _maxWidth, _maxHeight, _width, _height;
		private int _gridColumn, _gridRow, _gridColumnSpan = 1, _gridRowSpan = 1;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private LayoutState _layoutState = LayoutState.Invalid;
		private bool _measureDirty = true;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;
		private Point _lastLocationHint;

		private Rectangle _containerBounds;
		private Rectangle _bounds;
		private Rectangle _actualBounds;
		private Desktop _desktop;
		private bool _visible;

		private int _paddingLeft, _paddingRight, _paddingTop, _paddingBottom;
		private float _opacity = 1.0f;

		private bool _enabled;

		[DefaultValue(null)]
		public string Id { get; set; }

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
		public virtual bool Visible
		{
			get { return _visible; }

			set
			{
				if (_visible == value)
				{
					return;
				}

				_visible = value;

				OnVisibleChanged();
			}
		}

		internal bool Active
		{
			get; set;
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

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable Background { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable OverBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable DisabledBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable FocusedBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable DisabledOverBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable OverrideBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable Border { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable OverBorder { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable DisabledBorder { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable FocusedBorder { get; set; }

		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool ClipToBounds { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool IsMouseOver
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Bounds.Contains(Desktop.MousePosition);
			}
		}

		internal bool WasMouseOver
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Bounds.Contains(Desktop.LastMousePosition);
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsTouchOver
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Bounds.Contains(Desktop.TouchPosition);
			}
		}

		internal bool WasTouchOver
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Bounds.Contains(Desktop.LastTouchPosition);
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public virtual Desktop Desktop
		{
			get { return _desktop; }

			set
			{
				_desktop = value;
			}
		}

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


		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsKeyboardFocused
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Desktop.FocusedKeyboardWidget == this;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsMouseWheelFocused
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Desktop.FocusedMouseWheelWidget == this;
			}
		}

		protected virtual bool UseHoverRenderable
		{
			get
			{
				return IsMouseOver && Active;
			}
		}

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

		public Widget()
		{
			Visible = true;
			Enabled = true;
		}

		public virtual IRenderable GetCurrentBackground()
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
				if (!Enabled && DisabledOverBackground != null)
				{
					result = DisabledOverBackground;
				}
				else if (Enabled && OverBackground != null)
				{
					result = OverBackground;
				}
			}

			return result;
		}

		public virtual IRenderable GetCurrentBorder()
		{
			var result = Border;
			if (!Enabled && DisabledBorder != null)
			{
				result = DisabledBorder;
			}
			else if (Enabled && IsKeyboardFocused && FocusedBorder != null)
			{
				result = FocusedBorder;
			}
			else if (UseHoverRenderable && OverBorder != null)
			{
				result = OverBorder;
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

			// Border
			var border = GetCurrentBorder();
			if (border != null)
			{
				context.Draw(border, Bounds);
			}

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
				if (Width != null && availableSize.X > Width.Value)
				{
					availableSize.X = Width.Value;
				}
				else if (MaxWidth != null && availableSize.X > MaxWidth.Value)
				{
					availableSize.X = MaxWidth.Value;
				}

				if (Height != null && availableSize.Y > Height.Value)
				{
					availableSize.Y = Height.Value;
				}
				else if (MaxHeight != null && availableSize.Y > MaxHeight.Value)
				{
					availableSize.Y = MaxHeight.Value;
				}

				// Do the actual measure
				result = InternalMeasure(availableSize);

				// Result lerp
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

				// Align
				var controlBounds = LayoutUtils.Align(_containerBounds.Size(), size, HorizontalAlignment, VerticalAlignment);
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
				throw new Exception(string.Format("Could not find widget with id {0}", id));
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

			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;

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
			MouseLeft.Invoke(this);
		}

		public virtual void OnMouseEntered()
		{
			MouseEntered.Invoke(this);
		}

		public virtual void OnMouseMoved()
		{
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
			TouchLeft.Invoke(this);
		}

		public virtual void OnTouchEntered()
		{
			TouchEntered.Invoke(this);
		}

		public virtual void OnTouchMoved()
		{
			TouchMoved.Invoke(this);
		}

		public virtual void OnTouchDown()
		{
			TouchDown.Invoke(this);
		}

		public virtual void OnTouchUp()
		{
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
			if (Desktop == null)
			{
				return;
			}

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

			if (IsTouchOver)
			{
				OnTouchDoubleClick();
			}
		}

		internal bool HandleMouseMovement()
		{
			var isMouseOver = IsMouseOver;
			var wasMouseOver = WasMouseOver;

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

			return IsMouseOver;
		}

		internal void HandleTouchDown()
		{
			if (!Visible)
			{
				return;
			}

			if (IsTouchOver)
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

			if (IsTouchOver)
			{
				OnTouchUp();
			}
		}

		internal bool HandleTouchMovement()
		{
			var isTouchOver = IsTouchOver;
			var wasTouchOver = WasTouchOver;

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

			return IsTouchOver;
		}
	}
}