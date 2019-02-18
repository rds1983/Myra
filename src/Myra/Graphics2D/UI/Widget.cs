using System;
using System.ComponentModel;
using Myra.Attributes;
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

		public const string DefaultStyleName = "default";

		private int _left, _top;
		private int? _width, _height;
		private int _gridColumn, _gridRow, _gridColumnSpan = 1, _gridRowSpan = 1;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private LayoutState _layoutState = LayoutState.Invalid;
		private bool _measureDirty = true;
		private bool _isMouseOver = false;

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
		private bool _canFocus;
		private string _styleName;

		[DefaultValue(null)]
		public string Id { get; set; }

		[Selection(typeof(StyleNamesProvider))]
		public string StyleName
		{
			get { return _styleName; }
			set
			{
				if (value == _styleName)
				{
					return;
				}

				_styleName = value;
				SetStyleByName(Stylesheet.Current, value);
			}
		}

		[EditCategory("Layout")]
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

		[Obsolete("Use Left instead")]
		[HiddenInEditor]
		public int XHint
		{
			get
			{
				return Left;
			}

			set
			{
				Left = value;
			}
		}

		[EditCategory("Layout")]
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

		[Obsolete("Use Top instead")]
		[HiddenInEditor]
		public int YHint
		{
			get
			{
				return Top;
			}

			set
			{
				Top = value;
			}
		}

		[EditCategory("Layout")]
		[DefaultValue(null)]
		public int? Width
		{
			get { return _width; }
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

		[Obsolete("Use Width instead")]
		[HiddenInEditor]
		public int? WidthHint
		{
			get
			{
				return Width;
			}

			set
			{
				Width = value;
			}
		}

		[EditCategory("Layout")]
		[DefaultValue(null)]
		public int? Height
		{
			get { return _height; }
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

		[Obsolete("Use Height instead")]
		[HiddenInEditor]
		public int? HeightHint
		{
			get
			{
				return Height;
			}

			set
			{
				Height = value;
			}
		}

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[EditCategory("Layout")]
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

		[Obsolete("Use GridColumn")]
		[HiddenInEditor]
		public int GridPositionX
		{
			get
			{
				return GridColumn;
			}

			set
			{
				GridColumn = value;
			}
		}

		[EditCategory("Layout")]
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

		[Obsolete("Use GridRow")]
		[HiddenInEditor]
		public int GridPositionY
		{
			get
			{
				return GridRow;
			}

			set
			{
				GridRow = value;
			}
		}

		[EditCategory("Layout")]
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

		[Obsolete("Use GridColumnSpan")]
		[HiddenInEditor]
		public int GridSpanX
		{
			get
			{
				return GridColumnSpan;
			}

			set
			{
				GridColumnSpan = value;
			}
		}

		[EditCategory("Layout")]
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

		[Obsolete("Use GridRowSpan")]
		[HiddenInEditor]
		public int GridSpanY
		{
			get
			{
				return GridRowSpan;
			}

			set
			{
				GridRowSpan = value;
			}
		}

		[EditCategory("Behavior")]
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

				var ev = EnabledChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[EditCategory("Behavior")]
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

		[EditCategory("Appearance")]
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

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Background { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable FocusedBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledOverBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverrideBackground { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Border { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverBorder { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledBorder { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable FocusedBorder { get; set; }

		[EditCategory("Appearance")]
		[DefaultValue(false)]
		public virtual bool ClipToBounds { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public bool IsMouseOver
		{
			get
			{
				return _isMouseOver;
			}

			set
			{
				if (value == _isMouseOver)
				{
					return;
				}

				_isMouseOver = value;

				if (value)
				{
					OnMouseEntered();
				}
				else
				{
					OnMouseLeft();
				}
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Point MousePosition
		{
			get
			{
				return Desktop.MousePosition;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public virtual Desktop Desktop
		{
			get { return _desktop; }

			set
			{
				if (CanFocus && _desktop != null)
				{
					_desktop.RemoveFocusableWidget(this);
				}

				_desktop = value;

				if (CanFocus && _desktop != null)
				{
					_desktop.AddFocusableWidget(this);
				}
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Container Parent { get; internal set; }

		[HiddenInEditor]
		[XmlIgnore]
		public object Tag { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Rectangle Bounds
		{
			get
			{
				return _bounds;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Rectangle ActualBounds
		{
			get
			{
				return _actualBounds;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Rectangle ContainerBounds
		{
			get
			{
				return _containerBounds;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int PaddingWidth
		{
			get { return _paddingLeft + _paddingRight; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int PaddingHeight
		{
			get { return _paddingTop + _paddingBottom; }
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public virtual bool CanFocus
		{
			get
			{
				return _canFocus;
			}

			set
			{
				if (_canFocus == value)
				{
					return;
				}

				// If old value was true, remove from focusable widgets
				if (_canFocus && Desktop != null)
				{
					Desktop.RemoveFocusableWidget(this);
				}

				_canFocus = value;

				// If new value is true, add to focusable widgets
				if (_canFocus && Desktop != null)
				{
					Desktop.AddFocusableWidget(this);
				}

			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public virtual bool IsFocused { get; internal set; }

		public event EventHandler VisibleChanged;
		public event EventHandler MeasureChanged;
		public event EventHandler EnabledChanged;

		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;
		public event EventHandler LayoutUpdated;

		public event EventHandler MouseLeft;
		public event EventHandler MouseEntered;
		public event EventHandler MouseMoved;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseDown;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseUp;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseDoubleClick;

		public event EventHandler TouchDown;
		public event EventHandler TouchUp;

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
			else if (Enabled && IsFocused && FocusedBackground != null)
			{
				result = FocusedBackground;
			}
			else if (IsMouseOver && OverBackground != null)
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
			else if (Enabled && IsFocused && FocusedBorder != null)
			{
				result = FocusedBorder;
			}
			else if (IsMouseOver && OverBorder != null)
			{
				result = OverBorder;
			}

			return result;
		}

		public virtual void Render(RenderContext context)
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

			if (MyraEnvironment.DrawFocusedWidgetFrame && IsFocused)
			{
				batch.DrawRectangle(Bounds, Color.Red);
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
				result = InternalMeasure(availableSize);
				if (Width.HasValue)
				{
					result.X = Width.Value;
				}

				if (Height.HasValue)
				{
					result.Y = Height.Value;
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

			var ev = LayoutUpdated;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
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
				throw new Exception(string.Format("Could not find widget with id {0}", id));
			}

			return result;
		}

		public virtual void InvalidateMeasure()
		{
			_measureDirty = true;

			InvalidateLayout();

			var ev = MeasureChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public void ApplyWidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;

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

		protected virtual void SetStyleByName(Stylesheet stylesheet, string name)
		{
		}

		internal virtual string[] GetStyleNames(Stylesheet stylesheet)
		{
			return null;
		}

		public virtual void ApplyStylesheet(Stylesheet stylesheet)
		{
			var styleName = string.IsNullOrEmpty(StyleName) ? Stylesheet.DefaultStyleName : StyleName;

			SetStyleByName(stylesheet, styleName);
		}

		public virtual void OnMouseLeft()
		{
			var ev = MouseLeft;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnMouseEntered()
		{
			var ev = MouseEntered;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnMouseMoved()
		{
			var ev = MouseMoved;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnMouseDown(MouseButtons mb)
		{
			var ev = MouseDown;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<MouseButtons>(mb));
			}
		}

		public virtual void OnMouseDoubleClick(MouseButtons mb)
		{
			var ev = MouseDoubleClick;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<MouseButtons>(mb));
			}
		}

		public virtual void OnMouseUp(MouseButtons mb)
		{
			var ev = MouseUp;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<MouseButtons>(mb));
			}
		}

		public virtual void OnMouseWheel(float delta)
		{
			var ev = MouseWheelChanged;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<float>(delta));
			}
		}

		public virtual void OnTouchDown()
		{
			var ev = TouchDown;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnTouchUp()
		{
			var ev = TouchUp;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnKeyDown(Keys k)
		{
			var ev = KeyDown;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<Keys>(k));
			}
		}

		public virtual void OnKeyUp(Keys k)
		{
			var ev = KeyUp;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<Keys>(k));
			}
		}

		public virtual void OnChar(char c)
		{
			var ev = Char;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<char>(c));
			}
		}

		public virtual void OnVisibleChanged()
		{
			// Visibility change can generate MouseEnter/MouseLeft events
			HandleMouseMovement();

			var ev = VisibleChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
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

		internal void IterateFocusable(Action<Widget> action)
		{
			Widget w = this;
			while (w != null)
			{
				if (w.CanFocus)
				{
					action(w);
				}
				w = w.Parent;
			}
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
			var ev = LocationChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private void FireSizeChanged()
		{
			var ev = SizeChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		internal void HandleMouseDown(MouseButtons buttons)
		{
			if (!Visible)
			{
				return;
			}

			if (IsMouseOver)
			{
				OnMouseDown(buttons);
			}
		}

		internal void HandleMouseDoubleClick(MouseButtons buttons)
		{
			if (!Visible)
			{
				return;
			}

			if (IsMouseOver)
			{
				OnMouseDoubleClick(buttons);
			}
		}

		internal void HandleMouseUp(MouseButtons buttons)
		{
			if (!Visible)
			{
				return;
			}

			if (IsMouseOver)
			{
				OnMouseUp(buttons);
			}
		}

		internal void HandleMouseMovement()
		{
			var wasMouseOver = IsMouseOver;

			IsMouseOver = Visible && Desktop != null && Bounds.Contains(Desktop.MousePosition);
			if (IsMouseOver)
			{
				if (wasMouseOver)
				{
					// Already inside
					OnMouseMoved();
				}
			}
		}

		internal void HandleTouchDown()
		{
			if (!Visible)
			{
				return;
			}

			if (IsMouseOver)
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

			if (IsMouseOver)
			{
				OnTouchUp();
			}
		}
	}
}