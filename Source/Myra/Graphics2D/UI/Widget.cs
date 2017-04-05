using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Widget
	{
		public const string DefaultStyleName = "default";
		public const int DoubleClickIntervalInMs = 500;

		private int _xHint, _yHint;
		private int? _widthHint, _heightHint;
		private int _gridPositionX, _gridPositionY, _gridSpanX = 1, _gridSpanY = 1;
		private HorizontalAlignment _horizontalAlignment;
		private VerticalAlignment _verticalAlignment;
		private bool _layoutDirty = true;
		private bool _arrangeDirty = true;
		private bool _measureDirty = true;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;

		private MouseButtons? _mouseButtonsDown;

		private Rectangle _containerBounds;
		private Rectangle _bounds;
		private Desktop _desktop;
		private bool _visible;

		private int _paddingLeft, _paddingRight, _paddingTop, _paddingBottom;

		private DateTime _lastDown;

		public static bool DrawFrames { get; set; }
		public static bool DrawFocused { get; set; }

		public string Id { get; set; }

		[EditCategory("Layout")]
		public int XHint
		{
			get { return _xHint; }

			set
			{
				if (value == _xHint)
				{
					return;
				}

				_xHint = value;
				InvalidateLayout(false);
			}
		}

		[EditCategory("Layout")]
		public int YHint
		{
			get { return _yHint; }

			set
			{
				if (value == _yHint)
				{
					return;
				}

				_yHint = value;
				InvalidateLayout(false);
			}
		}

		[EditCategory("Layout")]
		public int? WidthHint
		{
			get { return _widthHint; }
			set
			{
				if (value == _widthHint)
				{
					return;
				}

				_widthHint = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		public int? HeightHint
		{
			get { return _heightHint; }
			set
			{
				if (value == _heightHint)
				{
					return;
				}

				_heightHint = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
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
		public HorizontalAlignment HorizontalAlignment
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
		public VerticalAlignment VerticalAlignment
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
		public int GridPositionX
		{
			get { return _gridPositionX; }

			set
			{
				if (value == _gridPositionX)
				{
					return;
				}

				_gridPositionX = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		public int GridPositionY
		{
			get { return _gridPositionY; }

			set
			{
				if (value == _gridPositionY)
				{
					return;
				}

				_gridPositionY = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		public int GridSpanX
		{
			get { return _gridSpanX; }

			set
			{
				if (value == _gridSpanX)
				{
					return;
				}

				_gridSpanX = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Layout")]
		public int GridSpanY
		{
			get { return _gridSpanY; }

			set
			{
				if (value == _gridSpanY)
				{
					return;
				}

				_gridSpanY = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Behavior")]
		public virtual bool Enabled { get; set; }

		[EditCategory("Behavior")]
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

				var ev = VisibleChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D Background { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D Border { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D OverBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D OverBorder { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D DisabledBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D DisabledOverBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public TextureRegion2D DisabledBorder { get; set; }

		[EditCategory("Appearance")]
		public bool ClipToBounds { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public bool IsMouseOver { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public MouseButtons? MouseButtonsDown
		{
			get { return _mouseButtonsDown; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Desktop Desktop
		{
			get { return _desktop; }

			set
			{
				if (value == _desktop)
				{
					return;
				}

				OnDesktopChanging();
				_desktop = value;
				OnDesktopChanged();
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
		public Point Size
		{
			get { return _bounds.Size(); }
			private set
			{
				if (value == _bounds.Size())
				{
					return;
				}

				GraphicsExtension.SetSize(ref _bounds, value);
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public virtual Point AbsoluteLocation { get; internal set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Point Location
		{
			get { return _bounds.Location; }
			private set
			{
				if (value == _bounds.Location)
				{
					return;
				}

				_bounds.Location = value;

				if (Parent != null)
				{
					AbsoluteLocation = Parent.AbsoluteLocation + Location;
				}
				else
				{
					AbsoluteLocation = Location;
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle Bounds
		{
			get { return _bounds; }

			internal set
			{
				Location = value.Location;
				Size = value.Size();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle AbsoluteBounds
		{
			get { return new Rectangle(AbsoluteLocation.X, AbsoluteLocation.Y, _bounds.Width, _bounds.Height); }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle LayoutBounds
		{
			get { return CalculateClientBounds(new Rectangle(0, 0, _bounds.Width, _bounds.Height)); }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle RenderBounds
		{
			get
			{
				return CalculateClientBounds(AbsoluteBounds);
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
		public bool CanFocus { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public virtual bool AcceptsTab
		{
			get { return false; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public virtual bool IsFocused { get; internal set; }

		public event EventHandler VisibleChanged;
		public event EventHandler MeasureChanged;

		public event EventHandler MouseLeft;
		public event EventHandler<GenericEventArgs<Point>> MouseEntered;
		public event EventHandler<GenericEventArgs<Point>> MouseMoved;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseDown;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseUp;
		public event EventHandler<GenericEventArgs<MouseButtons>> DoubleClick;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<char>> KeyPressed;
		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;

		public Widget()
		{
			Visible = true;
			Enabled = true;
		}

		public virtual TextureRegion2D GetCurrentBackground()
		{
			var result = Background;

			if (!Enabled && DisabledBackground != null)
			{
				result = DisabledBackground;
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

		public virtual TextureRegion2D GetCurrentBorder()
		{
			var result = Border;
			if (!Enabled && DisabledBorder != null)
			{
				result = DisabledBorder;
			}
			else if (IsMouseOver && OverBorder != null)
			{
				result = OverBorder;
			}

			return result;
		}

		public virtual void Render(SpriteBatch batch)
		{
			var bounds = AbsoluteBounds;

			if (bounds.Width == 0 || bounds.Height == 0)
			{
				return;
			}

			var oldScissorRectangle = batch.GraphicsDevice.ScissorRectangle;
			if (ClipToBounds)
			{
				oldScissorRectangle = batch.GraphicsDevice.ScissorRectangle;
				var newScissorRectangle = Rectangle.Intersect(oldScissorRectangle, bounds);

				if (newScissorRectangle.IsEmpty)
				{
					return;
				}

				batch.FlushUI();
				batch.GraphicsDevice.ScissorRectangle = newScissorRectangle;
			}

			UpdateLayout();

			// Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				batch.Draw(background, bounds);
			}

			InternalRender(batch);

			// Border
			var border = GetCurrentBorder();
			if (border != null)
			{
				batch.Draw(border, bounds);
			}

			if (DrawFrames)
			{
				batch.DrawRectangle(bounds, Color.LightGreen);
			}

			if (DrawFocused && IsFocused)
			{
				batch.DrawRectangle(bounds, Color.Red);
			}

			if (ClipToBounds)
			{
				batch.FlushUI();
				batch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
			}
		}

		public virtual void InternalRender(SpriteBatch batch)
		{
		}

		public Point Measure(Point availableSize)
		{
			if (!_measureDirty && _lastMeasureAvailableSize == availableSize)
			{
				return _lastMeasureSize;
			}

			Point result;

			if (WidthHint.HasValue && HeightHint.HasValue)
			{
				result = new Point(WidthHint.Value, HeightHint.Value);
			}
			else
			{
				result = InternalMeasure(availableSize);
				if (WidthHint.HasValue)
				{
					result.X = WidthHint.Value;
				}

				if (HeightHint.HasValue)
				{
					result.Y = HeightHint.Value;
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

		internal void Layout(Rectangle containerBounds)
		{
			if (_containerBounds == containerBounds)
			{
				return;
			}

			_containerBounds = containerBounds;
			InvalidateLayout();
			UpdateLayout();
		}

		public void InvalidateLayout(bool invalidateArrange = true)
		{
			if (invalidateArrange)
			{
				_arrangeDirty = true;
			}
			_layoutDirty = true;
		}

		public virtual void UpdateLayout()
		{
			if (!_layoutDirty)
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

			// Align
			var controlBounds = LayoutUtils.Align(_containerBounds.Size(), size, HorizontalAlignment, VerticalAlignment);
			controlBounds.Offset(_containerBounds.Location);

			controlBounds.Offset(XHint, YHint);

			Bounds = controlBounds;

			if (_arrangeDirty)
			{
				Arrange();
				_arrangeDirty = false;
			}

			_layoutDirty = false;
		}

		public virtual void OnDesktopChanging()
		{
		}

		public virtual void OnDesktopChanged()
		{
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
				foreach (var widget in asContainer.Children)
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
			WidthHint = style.WidthHint;
			HeightHint = style.HeightHint;

			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;

			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;

			PaddingLeft = style.Padding.Left;
			PaddingRight = style.Padding.Right;
			PaddingTop = style.Padding.Top;
			PaddingBottom = style.Padding.Bottom;
		}

		public virtual void OnMouseLeft()
		{
			IsMouseOver = false;

			var ev = MouseLeft;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void OnMouseEntered(Point position)
		{
			IsMouseOver = true;

			var ev = MouseEntered;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<Point>(position));
			}
		}

		public virtual void OnMouseMoved(Point position)
		{
			var ev = MouseMoved;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<Point>(position));
			}
		}

		public virtual void OnMouseDown(MouseButtons mb)
		{
			_mouseButtonsDown = mb;

			var ev = MouseDown;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<MouseButtons>(mb));
			}

			if ((DateTime.Now - _lastDown).TotalMilliseconds < DoubleClickIntervalInMs)
			{
				// Double click
				var ev2 = DoubleClick;
				if (ev2 != null)
				{
					ev2(this, new GenericEventArgs<MouseButtons>(mb));
				}

				_lastDown = DateTime.MinValue;
			}
			else
			{
				_lastDown = DateTime.Now;
			}
		}

		public virtual void OnMouseUp(MouseButtons mb)
		{
			_mouseButtonsDown = null;

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
	}
}