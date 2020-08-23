using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.Properties;
using Myra.Attributes;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#else
using Stride.Core.Mathematics;
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class Widget : BaseObject
	{
		private enum LayoutState
		{
			Normal,
			LocationInvalid,
			Invalid
		}

		private Point? _startPos;
		private Thickness _margin, _borderThickness, _padding;
		private int _left, _top;
		private int? _minWidth, _minHeight, _maxWidth, _maxHeight, _width, _height;
		private int _gridColumn, _gridRow, _gridColumnSpan = 1, _gridRowSpan = 1;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private LayoutState _layoutState = LayoutState.Invalid;
		private bool _isModal = false;
		private bool _measureDirty = true;
		private bool _active = false;
		private Desktop _desktop;
		private bool _isDraggable = false;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;
		private Point _lastLocationHint;

		private Rectangle _containerBounds;
		private Rectangle _bounds;
		private Rectangle _actualBounds;
		private bool _visible;

		private float _opacity = 1.0f;

		private bool _enabled;
		private bool _isKeyboardFocused = false;

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

		[Obsolete("Use Padding")]
		[Browsable(false)]
		public int PaddingLeft
		{
			get
			{
				return Padding.Left;
			}

			set
			{
				var p = Padding;
				p.Left = value;
				Padding = p;
			}
		}


		[Obsolete("Use Padding")]
		[Browsable(false)]
		public int PaddingRight
		{
			get
			{
				return Padding.Right;
			}

			set
			{
				var p = Padding;
				p.Right = value;
				Padding = p;
			}
		}

		[Obsolete("Use Padding")]
		[Browsable(false)]
		public int PaddingTop
		{
			get
			{
				return Padding.Top;
			}

			set
			{
				var p = Padding;
				p.Top = value;
				Padding = p;
			}
		}

		[Obsolete("Use Padding")]
		[Browsable(false)]
		public int PaddingBottom
		{
			get
			{
				return Padding.Top;
			}

			set
			{
				var p = Padding;
				p.Bottom = value;
				Padding = p;
			}
		}

		[Category("Layout")]
		[DesignerFolded]
		public Thickness Margin
		{
			get
			{
				return _margin;
			}

			set
			{
				if (_margin == value)
				{
					return;
				}

				_margin = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		[DesignerFolded]
		public Thickness BorderThickness
		{
			get
			{
				return _borderThickness;
			}

			set
			{
				if (_borderThickness == value)
				{
					return;
				}

				_borderThickness = value;
				InvalidateMeasure();
			}
		}

		[Category("Layout")]
		[DesignerFolded]
		public Thickness Padding
		{
			get
			{
				return _padding;
			}

			set
			{
				if (_padding == value)
				{
					return;
				}

				_padding = value;
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
				IsMouseInside = false;
				IsTouchInside = false;

				OnVisibleChanged();
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool IsDraggable
		{
			get
			{
				return _isDraggable;
			}

			set
			{
				if (value == _isDraggable)
				{
					return;
				}

				_isDraggable = value;
				SubscribeOnTouchMoved(IsPlaced && IsDraggable);
			}
		}

		[Category("Behavior")]
		[DefaultValue(DragDirection.Both)]
		public DragDirection DragDirection { get; set; } = DragDirection.Both;

		[XmlIgnore]
		[Browsable(false)]
		public Widget DragHandle { get; set; }
		
		
		[XmlIgnore]
		[Browsable(false)]
		private int RelativeLeft { get; set; }
		
		[XmlIgnore]
		[Browsable(false)]
		private int RelativeTop { get; set; }
		
		[XmlIgnore]
		[Browsable(false)]

		private int RelativeRight { get; set; }
		
		[XmlIgnore]
		[Browsable(false)]
		private int RelativeBottom { get; set; }

		/// <summary>
		/// Determines whether the widget had been placed on Desktop
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public bool IsPlaced
		{
			get
			{
				return Desktop != null;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public virtual Desktop Desktop
		{
			get
			{
				return _desktop;
			}

			internal set
			{
				_desktop = value;
				IsMouseInside = false;
				IsTouchInside = false;

				if (_desktop != null)
				{
					InvalidateLayout();
				}

				SubscribeOnTouchMoved(IsPlaced && IsDraggable);
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

		/// <summary>
		/// Dynamic layout expression
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
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
		public IBrush Border
		{
			get; set;
		}

		[Category("Appearance")]
		public IBrush OverBorder
		{
			get; set;
		}

		[Category("Appearance")]
		public IBrush DisabledBorder
		{
			get; set;
		}

		[Category("Appearance")]
		public IBrush FocusedBorder
		{
			get; set;
		}

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

		internal Rectangle BorderBounds
		{
			get
			{
				return _bounds - _margin;
			}
		}

		internal bool ContainsMouse
		{
			get
			{
				return Desktop != null && BorderBounds.Contains(Desktop.MousePosition);
			}
		}

		internal bool ContainsTouch
		{
			get
			{
				return Desktop != null && BorderBounds.Contains(Desktop.TouchPosition);
			}
		}

		protected Rectangle BackgroundBounds
		{
			get
			{
				return BorderBounds - _borderThickness;
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
		public int MBPWidth
		{
			get
			{
				return Margin.Left + Margin.Right +
					BorderThickness.Left + BorderThickness.Right +
					Padding.Left + Padding.Right;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int MBPHeight
		{
			get
			{
				return Margin.Top + Margin.Bottom +
					BorderThickness.Top + BorderThickness.Bottom +
					Padding.Top + Padding.Bottom;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool AcceptsKeyboardFocus { get; set; }

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
		public bool IsKeyboardFocused
		{
			get
			{
				return _isKeyboardFocused;
			}

			internal set
			{
				if (value == _isKeyboardFocused)
				{
					return;
				}

				_isKeyboardFocused = value;
				KeyboardFocusChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsMouseWheelFocused
		{
			get
			{
				return Desktop != null && Desktop.FocusedMouseWheelWidget == this;
			}
		}

		protected virtual bool UseHoverRenderable
		{
			get
			{
				return IsMouseInside && Active;
			}
		}

		public event EventHandler VisibleChanged;
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

		public event EventHandler KeyboardFocusChanged;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;
		public event EventHandler<GenericEventArgs<char>> Char;

		[Browsable(false)]
		[XmlIgnore]
		public Action<RenderContext> BeforeRender, AfterRender;

		public Widget()
		{
			Visible = true;
			Enabled = true;
		}

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

		public virtual IBrush GetCurrentBorder()
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

		public void BringToFront()
		{
			if (Parent != null && !(Parent is IMultipleItemsContainer))
			{
				return;
			}

			var widgets = (Parent as IMultipleItemsContainer)?.Widgets ?? Desktop.Widgets;

			if (widgets[widgets.Count - 1] == this) return;

			widgets.Remove(this);
			widgets.Add(this);
		}

		public void BringToBack()
		{
			if (Parent != null && !(Parent is IMultipleItemsContainer))
			{
				return;
			}

			var widgets = (Parent as IMultipleItemsContainer)?.Widgets ?? Desktop.Widgets;

			if (widgets[widgets.Count - 1] == this) return;

			widgets.Remove(this);
			widgets.Insert(0, this);
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

				if (context.SpriteBatchBeginParams.TransformMatrix.HasValue)
				{
					var pos = new Point(newScissorRectangle.X, newScissorRectangle.Y).ToVector2();
					var size = new Point(newScissorRectangle.Width, newScissorRectangle.Height).ToVector2();

					pos = Vector2.Transform(pos, context.SpriteBatchBeginParams.TransformMatrix.Value);
					size = Vector2.Transform(size, context.SpriteBatchBeginParams.TransformMatrix.Value);

					newScissorRectangle = new Rectangle(pos.ToPoint(), size.ToPoint());
				}

				CrossEngineStuff.SetScissor(newScissorRectangle);
			}

			var oldOpacity = context.Opacity;
			var oldView = context.View;

			context.Opacity *= Opacity;
			context.View = view;

			BeforeRender?.Invoke(context);

			// Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				context.Draw(background, BackgroundBounds);
			}

			// Borders
			var border = GetCurrentBorder();
			if (border != null)
			{
				var borderBounds = BorderBounds;
				if (BorderThickness.Left > 0)
				{
					context.Draw(border, new Rectangle(borderBounds.X, borderBounds.Y, BorderThickness.Left, borderBounds.Height));
				}

				if (BorderThickness.Top > 0)
				{
					context.Draw(border, new Rectangle(borderBounds.X, borderBounds.Y, borderBounds.Width, BorderThickness.Top));
				}

				if (BorderThickness.Right > 0)
				{
					context.Draw(border, new Rectangle(borderBounds.Right - BorderThickness.Right, borderBounds.Y, BorderThickness.Right, borderBounds.Height));
				}

				if (BorderThickness.Bottom > 0)
				{
					context.Draw(border, new Rectangle(borderBounds.X, borderBounds.Bottom - BorderThickness.Bottom, borderBounds.Width, BorderThickness.Bottom));
				}
			}

			InternalRender(context);

			AfterRender?.Invoke(context);

			// Restore context settings
			context.View = oldView;
			context.Opacity = oldOpacity;

			// Optional debug rendering
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
				// Restore scissor
				context.Flush();
				CrossEngineStuff.SetScissor(oldScissorRectangle);
			}

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

				availableSize.X -= MBPWidth;
				availableSize.Y -= MBPHeight;

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

			result.X += MBPWidth;
			result.Y += MBPHeight;

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

				if (HorizontalAlignment == HorizontalAlignment.Stretch && Width != null && Width.Value < containerSize.X)
				{
					containerSize.X = Width.Value;
				}

				if (VerticalAlignment == VerticalAlignment.Stretch && Height != null && Height.Value < containerSize.Y)
				{
					containerSize.Y = Height.Value;
				}

				// Align
				var controlBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment, Parent == null);
				controlBounds.Offset(_containerBounds.Location);

				controlBounds.Offset(Left, Top);

				_bounds = controlBounds;
				_actualBounds = CalculateClientBounds(controlBounds);

				Arrange();
				
				CalculateRelativePositions();
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

		private void CalculateRelativePositions()
		{
			RelativeLeft = Left - Bounds.X;
			RelativeTop = Top - Bounds.Y;
			
			if (Parent != null)
			{
				RelativeRight = Left + Parent.Bounds.Width - Bounds.X;
				RelativeBottom = Top + Parent.Bounds.Height - Bounds.Y;
			}
			else
			{
				RelativeRight = Left + Desktop.InternalBounds.Width - Bounds.X;
				RelativeBottom = Top + Desktop.InternalBounds.Height - Bounds.Y;
			}
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

			if (Parent != null)
			{
				Parent.InvalidateMeasure();
			}
			else if (Desktop != null)
			{
				Desktop.InvalidateLayout();
			}
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

			Margin = style.Margin;
			BorderThickness = style.BorderThickness;
			Padding = style.Padding;
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

			var x = Bounds.X;
			var y = Bounds.Y;

			var bounds = DragHandle != null
					? new Rectangle(
							x,
							y,
							DragHandle.Bounds.Right - x,
							DragHandle.Bounds.Bottom - y
					) : Rectangle.Empty;

			var touchPos = Desktop.TouchPosition;

			if (bounds == Rectangle.Empty || bounds.Contains(touchPos))
			{
				_startPos = new Point(touchPos.X - Left,
						touchPos.Y - Top);
			}

			TouchDown.Invoke(this);
		}

		public virtual void OnTouchUp()
		{
			_startPos = null;
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
			IsKeyboardFocused = false;
		}

		public virtual void OnGotKeyboardFocus()
		{
			IsKeyboardFocused = true;
		}

		protected internal virtual void OnActiveChanged()
		{
		}

		internal Rectangle CalculateClientBounds(Rectangle clientBounds)
		{
			return clientBounds - Margin - BorderThickness - Padding;
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

		public void SetKeyboardFocus()
		{
			Desktop.FocusedKeyboardWidget = this;
		}

		public void SetMouseWheelFocus()
		{
			Desktop.FocusedMouseWheelWidget = this;
		}

		private void SubscribeOnTouchMoved(bool subscribe)
		{
			if (Parent != null)
			{
				Parent.TouchMoved -= DesktopOnTouchMoved;
				Parent.TouchUp -= DesktopTouchUp;
			}
			else if (Desktop != null)
			{
				Desktop.TouchMoved -= DesktopOnTouchMoved;
				Desktop.TouchUp -= DesktopTouchUp;
			}

			if (subscribe)
			{
				if (Parent != null)
				{
					Parent.TouchMoved += DesktopOnTouchMoved;
					Parent.TouchUp += DesktopTouchUp;
				}
				else if (Desktop != null)
				{
					Desktop.TouchMoved += DesktopOnTouchMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		private void DesktopOnTouchMoved(object sender, EventArgs args)
		{
			if (_startPos == null || !IsDraggable)
			{
				return;
			}

			var position = new Point(Desktop.TouchPosition.X - _startPos.Value.X,
				Desktop.TouchPosition.Y - _startPos.Value.Y);

			int newLeft = Left;
			int newTop = Top;
			
			if (DragDirection.HasFlag(DragDirection.Horizontal))
			{
				newLeft = position.X;
			}

			if (DragDirection.HasFlag(DragDirection.Vertical))
			{
				newTop = position.Y;
			}

			ConstrainToBounds(ref newLeft, ref newTop);

			Left = newLeft;
			Top = newTop;
		}

		private void ConstrainToBounds(ref int newLeft, ref int newTop)
		{
			if (newLeft < RelativeLeft)
			{
				newLeft = RelativeLeft;
			}

			if (newTop < RelativeTop)
			{
				newTop = RelativeTop;
			}

			if (newTop + Bounds.Height > RelativeBottom)
			{
				newTop = RelativeBottom - Bounds.Height;
			}
				
			if (newLeft + Bounds.Width > RelativeRight)
			{
				newLeft = RelativeRight - Bounds.Width;
			}
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			_startPos = null;
		}
	}
}