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
using System.Numerics;
using System.Drawing;
using Myra.Platform;
using Matrix = System.Numerics.Matrix3x2;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	[Flags]
	public enum DragDirection
	{
		None = 0,
		Vertical = 1,
		Horizontal = 2,
		Both = Vertical | Horizontal
	}

	public partial class Widget : BaseObject, ITransformable
	{
		private MouseCursorType? _mouseCursorType;
		private Vector2? _startPos;
		private Point _startLeftTop;
		private Thickness _margin, _borderThickness, _padding;
		private int _left, _top;
		private int? _minWidth, _minHeight, _maxWidth, _maxHeight, _width, _height;
		private int _zIndex;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private bool _measureDirty = true;
		private bool _arrangeDirty = true;
		private Desktop _desktop;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;

		private Rectangle _containerBounds;
		private Rectangle _layoutBounds;
		private bool _visible;

		private float _opacity = 1.0f;

		private bool _enabled;
		private bool _isKeyboardFocused = false;
		private Vector2 _scale = Vector2.One;
		private Vector2 _transformOrigin = new Vector2(0.5f, 0.5f);
		private float _rotation = 0.0f;
		private Transform? _transform;
		private Matrix _inverseMatrix;
		private bool _inverseMatrixDirty = true;

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
				InvalidateTransform();
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
				InvalidateTransform();
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

		[Category("Layout")]
		public IBrush Border { get; set; }

		[Category("Layout")]
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

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumn/Grid.SetColumn")]

		public int GridColumn
		{
			get => Grid.GetColumn(this);
			set => Grid.SetColumn(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumn/Grid.SetColumn")]
		public int GridRow
		{
			get => Grid.GetRow(this);
			set => Grid.SetRow(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumnSpan/Grid.SetColumnSpan")]
		public int GridColumnSpan
		{
			get => Grid.GetColumnSpan(this);
			set => Grid.SetColumnSpan(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumnSpan/Grid.SetColumnSpan")]
		public int GridRowSpan
		{
			get => Grid.GetRowSpan(this);
			set => Grid.SetRowSpan(this, value);
		}

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool Enabled
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

				foreach (var item in ChildrenCopy)
				{
					item.Enabled = value;
				}

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
				LocalMousePosition = null;
				LocalTouchPosition = null;

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

		[Category("Behavior")]
		[DefaultValue(null)]
		public virtual MouseCursorType? MouseCursor
		{
			get => _mouseCursorType;
			set
			{
				if (value == _mouseCursorType)
				{
					return;
				}

				_mouseCursorType = value;
				foreach (var child in Children)
				{
					child.MouseCursor = value;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(null)]
		public string Tooltip { get; set; }


		[Category("Transform")]
		[DefaultValue("1, 1")]
		[DesignerFolded]
		public Vector2 Scale
		{
			get => _scale;
			set
			{
				if (value == _scale)
				{
					return;
				}

				_scale = value;
				InvalidateTransform();
			}

		}

		[Category("Transform")]
		[DefaultValue("0.5, 0.5")]
		[DesignerFolded]
		public Vector2 TransformOrigin
		{
			get => _transformOrigin;
			set
			{
				if (value == _transformOrigin)
				{
					return;
				}

				_transformOrigin = value;
				InvalidateTransform();
			}
		}

		[Category("Transform")]
		[DefaultValue(0.0f)]
		[DesignerFolded]
		public float Rotation
		{
			get => _rotation;

			set
			{
				if (value == _rotation)
				{
					return;
				}

				_rotation = value;
				InvalidateTransform();
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public Widget DragHandle { get; set; }

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

					if (_desktop.Tooltip != null && _desktop.Tooltip.Tag == this)
					{
						_desktop.HideTooltip();
					}
				}

				LocalMousePosition = null;
				LocalTouchPosition = null;

				_desktop = value;

				if (_desktop != null)
				{
					InvalidateMeasure();
				}

				SubscribeOnTouchMoved(IsPlaced && IsDraggable);

				foreach (var child in ChildrenCopy)
				{
					child.Desktop = value;
				}

				OnPlacedChanged();
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public bool IsModal { get; set; }

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
		public Widget Parent { get; internal set; }

		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		/// <summary>
		/// Zero-based bounds
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Rectangle Bounds => new Rectangle(0, 0, _layoutBounds.Width, _layoutBounds.Height);

		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ActualBounds => Bounds - _margin - _borderThickness - _padding;

		/// <summary>
		/// Bounds - Margin
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle BorderBounds => Bounds - _margin;

		[Browsable(false)]
		[XmlIgnore] protected Rectangle BackgroundBounds => BorderBounds - _borderThickness;

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

		internal Transform Transform
		{
			get
			{
				if (_transform == null)
				{
					var p = new Point(_layoutBounds.X + Left, _layoutBounds.Y + Top);

					var localTransform = new Transform(p.ToVector2(),
						TransformOrigin * _layoutBounds.Size().ToVector2(),
						Scale,
						Rotation * (float)Math.PI / 180);

					if (Parent != null)
					{
						var transform = Parent.Transform;
						transform.AddTransform(ref localTransform);
						_transform = transform;
					}
					else if (Desktop != null)
					{
						var transform = Desktop.Transform;
						transform.AddTransform(ref localTransform);
						_transform = transform;
					}
					else
					{
						_transform = localTransform;
					}
				}

				return _transform.Value;
			}
		}

		protected virtual bool UseOverBackground => IsMouseInside;

		[Browsable(false)]
		[XmlIgnore]
		public Action<RenderContext> BeforeRender, AfterRender;

		public Widget()
		{
			Visible = true;
			Enabled = true;
			DragHandle = this;

			Children.CollectionChanged += ChildrenOnCollectionChanged;
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
			else if (UseOverBackground && OverBackground != null)
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
			else if (IsMouseInside && OverBorder != null)
			{
				result = OverBorder;
			}

			return result;
		}

		public void BringToFront()
		{
			var widgets = Parent != null ? Parent.Children : Desktop.Widgets;

			if (widgets[widgets.Count - 1] == this) return;

			widgets.Remove(this);
			widgets.Add(this);
		}

		public void BringToBack()
		{
			var widgets = Parent != null ? Parent.Children : Desktop.Widgets;

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

			if (!string.IsNullOrEmpty(Tooltip) && (Desktop.Tooltip == null || Desktop.Tooltip.Tag != this) &&
				_lastMouseMovement != null && (DateTime.Now - _lastMouseMovement.Value).TotalMilliseconds > MyraEnvironment.TooltipDelayInMs)
			{
				var pos = Desktop.MousePosition;
				pos.X += MyraEnvironment.TooltipOffset.X;
				pos.Y += MyraEnvironment.TooltipOffset.Y;
				Desktop.ShowTooltip(this, pos);
				_lastMouseMovement = null;
			}

			UpdateArrange();

			var oldTransform = context.Transform;

			// Apply widget transforms
			context.Transform = Transform;

			Rectangle? oldScissorRectangle = null;
			if (ClipToBounds && context.Transform.Rotation == 0)
			{
				oldScissorRectangle = context.Scissor;
				var absoluteBounds = context.Transform.Apply(Bounds);
				var newScissorRectangle = Rectangle.Intersect(context.Scissor, absoluteBounds);

				if (newScissorRectangle.Width == 0 || newScissorRectangle.Height == 0)
				{
					context.Transform = oldTransform;
					return;
				}

				context.Scissor = newScissorRectangle;
			}

			var oldOpacity = context.Opacity;
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
			BeforeRender?.Invoke(context);
			InternalRender(context);
			AfterRender?.Invoke(context);

			if (oldScissorRectangle != null)
			{
				// Restore scissor
				context.Scissor = oldScissorRectangle.Value;
			}

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

			// Restore context settings
			context.Transform = oldTransform;
			context.Opacity = oldOpacity;
		}

		public virtual void InternalRender(RenderContext context)
		{
			foreach (var child in ChildrenCopy)
			{
				child.Render(context);
			}
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

			result.X += MBPWidth;
			result.Y += MBPHeight;

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

			_lastMeasureSize = result;
			_lastMeasureAvailableSize = availableSize;
			_measureDirty = false;

			return result;
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
			var layoutBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment);
			layoutBounds.Offset(_containerBounds.Location);

			_layoutBounds = layoutBounds;
			InvalidateTransform();

			InternalArrange();
			ArrangeUpdated.Invoke(this);

			_arrangeDirty = false;
		}

		protected virtual void InternalArrange()
		{
			if (ChildrenLayout == null)
			{
				return;
			}

			ChildrenLayout.Arrange(ChildrenCopy, ActualBounds);
		}

		protected virtual Point InternalMeasure(Point availableSize)
		{
			if (ChildrenLayout == null)
			{
				return Mathematics.PointZero;
			}

			return ChildrenLayout.Measure(ChildrenCopy, availableSize);
		}


		public void InvalidateArrange()
		{
			_arrangeDirty = true;
		}

		/// <summary>
		/// Find a widget by id
		/// Throws exception if not found
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Widget EnsureWidgetById(string id)
		{
			var result = FindChildById(id);
			if (result == null)
			{
				throw new Exception(string.Format($"Could not find widget with id {id}"));
			}

			return result;
		}

		internal virtual void InvalidateTransform()
		{
			_transform = null;
			_inverseMatrixDirty = true;

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
			}
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

		public void RemoveFromParent()
		{
			if (Parent == null)
			{
				return;
			}

			Parent.Children.Remove(this);
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
			if (_startPos == null || !IsDraggable || Desktop == null)
			{
				return;
			}

			var parent = Parent != null ? (ITransformable)Parent : Desktop;
			var newPos = parent.ToLocal(new Vector2(Desktop.TouchPosition.Value.X, Desktop.TouchPosition.Value.Y));
			var delta = newPos - _startPos.Value;

			var newLeft = Left;
			var newTop = Top;
			if (DragDirection.HasFlag(DragDirection.Horizontal))
			{
				newLeft = _startLeftTop.X + (int)delta.X;
			}

			if (DragDirection.HasFlag(DragDirection.Vertical))
			{
				newTop = _startLeftTop.Y + (int)delta.Y;
			}

			var parentBounds = Parent != null ? Parent.Bounds : Desktop.InternalBounds;
			if (newLeft < 0)
			{
				newLeft = 0;
			}

			if (newLeft + Bounds.Width > parentBounds.Width)
			{
				newLeft = parentBounds.Width - Bounds.Width;
			}

			if (newTop < 0)
			{
				newTop = 0;
			}

			if (newTop + Bounds.Height > parentBounds.Height)
			{
				newTop = parentBounds.Height - Bounds.Height;
			}

			Left = newLeft;
			Top = newTop;
		}

		public Vector2 ToGlobal(Vector2 pos) => Transform.Apply(pos);

		public Point ToGlobal(Point pos) => Transform.Apply(pos);

		public Vector2 ToLocal(Vector2 source)
		{
			if (_inverseMatrixDirty)
			{
#if MONOGAME || FNA || STRIDE
				_inverseMatrix = Matrix.Invert(Transform.Matrix);
#else
				Matrix inverse = Matrix.Identity;
				Matrix.Invert(Transform.Matrix, out inverse);
				_inverseMatrix = inverse;
#endif
				_inverseMatrixDirty = false;
			}

			return source.Transform(ref _inverseMatrix);
		}

		public Point ToLocal(Point pos) => ToLocal(new Vector2(pos.X, pos.Y)).ToPoint();

		public bool ContainsGlobalPoint(Point globalPos)
		{
			var localPos = ToLocal(globalPos);
			return BorderBounds.Contains(localPos);
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			_startPos = null;
		}

		public virtual Widget HitTest(Point p)
		{
			if (Desktop == null || !Visible || !ContainsGlobalPoint(p))
			{
				return null;
			}

			Widget result = null;
			for (var i = _childrenCopy.Count - 1; i >= 0; i--)
			{
				var child = _childrenCopy[i];
				result = child.HitTest(p);
				if (result != null)
				{
					break;
				}
			}

			var localPos = ToLocal(p);
			if (result == null && !InputFallsThrough(localPos))
			{
				result = this;
			}

			return result;
		}

		public virtual bool InputFallsThrough(Point localPos) => false;

		public Widget Clone()
		{
			// Firstly try to use parameterless constructor
			var type = GetType();
			var constructor = type.GetConstructor(Type.EmptyTypes);

			Widget result;
			if (constructor != null)
			{
				result = (Widget)constructor.Invoke(new object[0]);
			}
			else
			{
				// Then string constructor
				result = (Widget)Activator.CreateInstance(GetType(), (string)null);
			}

			result.CopyFrom(this);

			// Copy attached properties
			foreach(var pair in AttachedPropertiesValues)
			{
				result.AttachedPropertiesValues[pair.Key] = pair.Value;
			}

			return result;
		}

		protected internal virtual void CopyFrom(Widget w)
		{
			StyleName = w.StyleName;
			Left = w.Left;
			Top = w.Top;
			MinWidth = w.MinWidth;
			MaxWidth = w.MaxWidth;
			Width = w.Width;
			MinHeight = w.MinHeight;
			MaxHeight = w.MaxHeight;
			Height = w.Height;
			Margin = w.Margin;
			Border = w.Border;
			BorderThickness = w.BorderThickness;
			Padding = w.Padding;
			HorizontalAlignment = w.HorizontalAlignment;
			VerticalAlignment = w.VerticalAlignment;
			Enabled = w.Enabled;
			Visible = w.Visible;
			DragDirection = w.DragDirection;
			ZIndex = w.ZIndex;
			MouseCursor = w.MouseCursor;
			Tooltip = w.Tooltip;
			Scale = w.Scale;
			TransformOrigin = w.TransformOrigin;
			Rotation = w.Rotation;
			DragHandle = w.DragHandle;
			IsModal = w.IsModal;
			Opacity = w.Opacity;
			Background = w.Background;
			OverBackground = w.OverBackground;
			DisabledBackground = w.DisabledBackground;
			FocusedBackground = w.FocusedBackground;
			OverBorder = w.OverBorder;
			DisabledBorder = w.DisabledBorder;
			FocusedBorder = w.FocusedBorder;
			ClipToBounds = w.ClipToBounds;
			Tag = w.Tag;
			AcceptsKeyboardFocus = w.AcceptsKeyboardFocus;
			BeforeRender = w.BeforeRender;
			AfterRender = w.AfterRender;
		}
	}
}