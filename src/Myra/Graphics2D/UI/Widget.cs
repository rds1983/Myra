using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.Properties;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public enum MouseWheelFocusType
	{
		None,
		Hover,
		Focus
	}

	[Flags]
	public enum DragDirection
	{
		None = 0,
		Vertical = 1,
		Horizontal = 2,
		Both = Vertical | Horizontal
	}

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
		private int _zIndex;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private bool _isModal = false;
		private bool _measureDirty = true;
		private bool _arrangeDirty = true;
		private bool _active = false;
		private Desktop _desktop;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;

		private Rectangle _containerBounds;
		private Rectangle _layoutBounds;
		private Point _boxOffset, _actualSize;
		private bool _visible;

		private float _opacity = 1.0f;

		private bool _isMouseInside, _enabled;
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
		[DefaultValue(DragDirection.None)]
		public virtual DragDirection DragDirection { get; set; } = DragDirection.None;

		[XmlIgnore]
		[Browsable(false)]
		internal bool IsDraggable { get => DragDirection != DragDirection.None; }

		[Category("Behavior")]
		[DefaultValue(0)]
		public int ZIndex
		{
			get => _zIndex;
			set
			{
				if (value == _zIndex)
				{
					return;
				}

				_zIndex = value;
				InvalidateMeasure();
			}
		}

		[Category("Transform")]
		[DesignerFolded]
		public Vector2 Scale { get; set; } = Vector2.One;

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
				if (_desktop != null && value == null)
				{
					if (_desktop.FocusedKeyboardWidget == this)
					{
						_desktop.FocusedKeyboardWidget = null;
					}

					if (_desktop.MouseInsideWidget == this)
					{
						_desktop.MouseInsideWidget = null;
					}
				}

				_desktop = value;
				IsMouseInside = false;
				IsTouchInside = false;

				if (_desktop != null)
				{
					InvalidateMeasure();
				}

				SubscribeOnTouchMoved(IsPlaced && IsDraggable);
				OnPlacedChanged();
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
		public bool IsMouseInside
		{
			get => _isMouseInside;
			set
			{
				if (value && _isMouseInside)
				{
					if (Desktop != null)
					{
						Desktop.MouseInsideWidget = this;
					}

					OnMouseMoved();
				} else if (value && !_isMouseInside)
				{
					if (Desktop != null)
					{
						Desktop.MouseInsideWidget = this;
					}

					OnMouseEntered();
				} else if (!value && _isMouseInside)
				{
					if (Desktop != null && Desktop.MouseInsideWidget == this)
					{
						Desktop.MouseInsideWidget = null;
					}

					OnMouseLeft();
				}

				_isMouseInside = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsTouchInside { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public Container Parent { get; internal set; }

		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		/// <summary>
		/// Widget bounds relative to container
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Rectangle Bounds
		{
			get
			{
				var result = _layoutBounds;
				result.Offset(Left, Top);
				return result;
			}
		}

		/// <summary>
		/// Bounds - Margin
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle BorderBounds => new Rectangle(0, 0, _layoutBounds.Width, _layoutBounds.Height) - _margin;

		/// <summary>
		/// Widget bounds in absolute coordinates
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle AbsoluteBounds => new Rectangle(AbsoluteOffset.X, AbsoluteOffset.Y, _layoutBounds.Width, _layoutBounds.Height);

		/// <summary>
		/// AbsoluteBounds - Margin
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle AbsoluteBorderBounds => AbsoluteBounds - _margin;

		/// <summary>
		/// AbsoluteBounds - Margin - Border - Padding
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle AbsoluteActualBounds => AbsoluteBounds - _margin - _borderThickness - _padding;

		[Browsable(false)]
		[XmlIgnore]
		public Point AbsoluteOffset { get; private set; }

		internal bool ContainsMouse => Desktop != null && AbsoluteBorderBounds.Contains(Desktop.MousePosition);

		internal bool ContainsTouch => Desktop != null && AbsoluteBorderBounds.Contains(Desktop.TouchPosition);

		protected Rectangle BackgroundBounds => BorderBounds - _borderThickness;

		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ActualBounds => new Rectangle(0, 0, _actualSize.X, _actualSize.Y);

		[Browsable(false)]
		[XmlIgnore]
		public int ActualWidth => _actualSize.X;

		[Browsable(false)]
		[XmlIgnore]
		public int ActualHeight => _actualSize.Y;

		[Browsable(false)]
		[XmlIgnore]
		public Point ActualSize => _actualSize;


		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ContainerBounds => _containerBounds;

		[Browsable(false)]
		[XmlIgnore]
		public int MBPWidth => Margin.Left + Margin.Right +
					BorderThickness.Left + BorderThickness.Right +
					Padding.Left + Padding.Right;

		[Browsable(false)]
		[XmlIgnore]
		public int MBPHeight => Margin.Top + Margin.Bottom +
					BorderThickness.Top + BorderThickness.Bottom +
					Padding.Top + Padding.Bottom;

		/// <summary>
		/// Determines whether a widget accepts keyboard focus
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool AcceptsKeyboardFocus { get; set; }


		[Browsable(false)]
		[XmlIgnore]
		internal protected virtual MouseWheelFocusType MouseWheelFocusType => MouseWheelFocusType.None;

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

		protected virtual bool UseHoverRenderable
		{
			get
			{
				return IsMouseInside && Active;
			}
		}

		public event EventHandler PlacedChanged;
		public event EventHandler VisibleChanged;
		public event EventHandler EnabledChanged;

		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;
		public event EventHandler ArrangeUpdated;

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

			UpdateArrange();

			var oldTransform = context.Transform;

			// Apply widget transforms
			context.Transform.AddOffset(Bounds.Location);
			context.Transform.AddScale(Scale);

			var absoluteBounds = context.Transform.Apply(new Rectangle(0, 0, _layoutBounds.Width, _layoutBounds.Height));
			AbsoluteOffset = absoluteBounds.Location;

			var absoluteView = Rectangle.Intersect(context.AbsoluteView, absoluteBounds);
			if (absoluteView.Width == 0 || absoluteView.Height == 0)
			{
				context.Transform = oldTransform;
				return;
			}

			var oldScissorRectangle = context.Scissor;
			if (ClipToBounds)
			{
				context.Scissor = absoluteView;
			}

			var oldView = context.AbsoluteView;
			var oldOpacity = context.Opacity;

			context.AbsoluteView = absoluteView;
			context.AddOpacity(Opacity);

			// Draw Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				background.Draw(context, BackgroundBounds);
			}

			// Draw Borders
			var border = GetCurrentBorder();
			if (border != null)
			{
				var borderBounds = BorderBounds;
				if (BorderThickness.Left > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Y, BorderThickness.Left, borderBounds.Height));
				}

				if (BorderThickness.Top > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Y, borderBounds.Width, BorderThickness.Top));
				}

				if (BorderThickness.Right > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.Right - BorderThickness.Right, borderBounds.Y, BorderThickness.Right, borderBounds.Height));
				}

				if (BorderThickness.Bottom > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Bottom - BorderThickness.Bottom, borderBounds.Width, BorderThickness.Bottom));
				}
			}

			// Internal rendering
			context.Transform.AddOffset(_boxOffset);
			BeforeRender?.Invoke(context);
			InternalRender(context);
			AfterRender?.Invoke(context);

			if (ClipToBounds && !MyraEnvironment.DisableClipping)
			{
				// Restore scissor
				context.Scissor = oldScissorRectangle;
			}

			// Restore context settings
			context.Transform = oldTransform;
			context.AbsoluteView = oldView;
			context.Opacity = oldOpacity;

			// Optional debug rendering
			if (MyraEnvironment.DrawWidgetsFrames)
			{
				context.DrawRectangle(Bounds, Color.LightGreen);
			}

			if (MyraEnvironment.DrawKeyboardFocusedWidgetFrame && IsKeyboardFocused)
			{
				context.DrawRectangle(Bounds, Color.Red);
			}

			if (MyraEnvironment.DrawMouseHoveredWidgetFrame && IsMouseInside)
			{
				context.DrawRectangle(Bounds, Color.Yellow);
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
			// Previously I skipped this step if both Width & Height were set
			// However that raised an issue - custom InternalMeasure stuff(such as in Menu.InternalMeasure) was skipped as well
			// So now InternalMeasure is called every time
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

			result.X += MBPWidth;
			result.Y += MBPHeight;

			_lastMeasureSize = result;
			_lastMeasureAvailableSize = availableSize;
			_measureDirty = false;

			return result;
		}

		protected virtual Point InternalMeasure(Point availableSize)
		{
			return Mathematics.PointZero;
		}

		public void Arrange(Rectangle containerBounds)
		{
			if (!_arrangeDirty && _containerBounds == containerBounds)
			{
				return;
			}

			_arrangeDirty = true;
			_containerBounds = containerBounds;
			UpdateArrange();
		}

		public void UpdateArrange()
		{
			if (!_arrangeDirty)
			{
				return;
			}

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
			var layoutBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment, Parent == null);
			layoutBounds.Offset(_containerBounds.Location);

			_layoutBounds = layoutBounds;
			var actualBounds = new Rectangle(0, 0, layoutBounds.Width, layoutBounds.Height) - Margin - BorderThickness - Padding;
			_boxOffset = new Point(actualBounds.X, actualBounds.Y);
			_actualSize = new Point(actualBounds.Width, actualBounds.Height);

			CalculateRelativePositions();

			InternalArrange();
			ArrangeUpdated.Invoke(this);

			_arrangeDirty = false;
		}

		public virtual void InternalArrange()
		{
		}


		public void InvalidateArrange()
		{
			_arrangeDirty = true;
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

			InvalidateArrange();

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

			var x = AbsoluteBorderBounds.X;
			var y = AbsoluteBorderBounds.Y;

			var bounds = DragHandle != null
					? new Rectangle(
							x,
							y,
							DragHandle.AbsoluteBorderBounds.Right - x,
							DragHandle.AbsoluteBorderBounds.Bottom - y
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

		protected virtual void OnPlacedChanged()
		{
			PlacedChanged?.Invoke(this);
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