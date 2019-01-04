using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

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
		private int _gridPositionX, _gridPositionY, _gridSpanX = 1, _gridSpanY = 1;
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
		public int GridPositionX
		{
			get { return _gridPositionX; }

			set
			{
				if (value == _gridPositionX)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridPositionX = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		[DefaultValue(0)]
		public int GridPositionY
		{
			get { return _gridPositionY; }

			set
			{
				if (value == _gridPositionY)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridPositionY = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		[DefaultValue(1)]
		public int GridSpanX
		{
			get { return _gridSpanX; }

			set
			{
				if (value == _gridSpanX)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridSpanX = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		[DefaultValue(1)]
		public int GridSpanY
		{
			get { return _gridSpanY; }

			set
			{
				if (value == _gridSpanY)
				{
					return;
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_gridSpanY = value;
				InvalidateMeasure();
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

				var ev = VisibleChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
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
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable Background { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable FocusedBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledOverBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverrideBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable Border { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable OverBorder { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable DisabledBorder { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public IRenderable FocusedBorder { get; set; }

		[EditCategory("Appearance")]
		[DefaultValue(false)]
		public virtual bool ClipToBounds { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		internal bool WasMouseOver
		{
			get
			{
				if (Desktop == null)
				{
					return false;
				}

				return Bounds.Contains(Desktop.OldMousePosition);
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
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

		[HiddenInEditor]
		[JsonIgnore]
		public Point MousePosition
		{
			get
			{
				return Desktop.MousePosition;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
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
		[JsonIgnore]
		public Container Parent { get; internal set; }

		[HiddenInEditor]
		[JsonIgnore]
		public object Tag { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle Bounds
		{
			get
			{
				return _bounds;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle ActualBounds
		{
			get
			{
				return _actualBounds;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle ContainerBounds
		{
			get
			{
				return _containerBounds;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public int PaddingWidth
		{
			get { return _paddingLeft + _paddingRight; }
		}

		[HiddenInEditor]
		[JsonIgnore]
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
		[JsonIgnore]
		public virtual bool IsFocused { get; internal set; }

		public event EventHandler VisibleChanged;
		public event EventHandler MeasureChanged;
		public event EventHandler EnabledChanged;

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
			var oldScissorRectangle = batch.GraphicsDevice.ScissorRectangle;
			if (ClipToBounds && !MyraEnvironment.DisableClipping)
			{
				oldScissorRectangle = batch.GraphicsDevice.ScissorRectangle;
				var newScissorRectangle = Rectangle.Intersect(oldScissorRectangle, view);

				if (newScissorRectangle.IsEmpty)
				{
					return;
				}

				context.Flush();
				batch.GraphicsDevice.ScissorRectangle = newScissorRectangle;
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
				batch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
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
			_bounds.Location += delta;
			_actualBounds.Location += delta;
			_containerBounds.Location += delta;
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
			Width = style.WidthHint;
			Height = style.HeightHint;

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
	}
}