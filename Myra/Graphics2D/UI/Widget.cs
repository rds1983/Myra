using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Edit;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Widget
	{
		public const string DefaultStyleName = "default";

		private int _xHint, _yHint;
		private int? _widthHint, _heightHint;

		private Point _locationInParent;
		private MouseButtons? _mouseButtonsDown;

		private Rectangle _bounds;
		private Desktop _desktop;
		private bool _visible;
		private PaddingInfo _padding;

		public static bool DrawFrames { get; set; }
		public static bool DrawFocused { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		private bool LayoutInvalid { get; set; }

		public string Id { get; set; }

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
				FireMeasureChanged();
			}
		}

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
				FireMeasureChanged();
			}
		}

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
				FireMeasureChanged();
			}
		}

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
				FireMeasureChanged();
			}
		}

		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }

		public Point GridPosition;
		public Point GridSpan = new Point(1, 1);

		public PaddingInfo Padding
		{
			get { return _padding; }
			set {
				if (value == _padding)
				{
					return;
				}

				_padding = value;
				FireMeasureChanged();
			}
		}

		public bool Enabled { get; set; }

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
		public Drawable Background { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Drawable OverBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Drawable DisabledBackground { get; set; }

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
		public Point LocationInParent
		{
			get { return _locationInParent; }
		}
		
		[HiddenInEditor]
		[JsonIgnore]
		public Point Size
		{
			get { return _bounds.Size; }
			internal set
			{
				if (value == _bounds.Size)
				{
					return;
				}

				_bounds.Size = value;
				Arrange();
				FireSizeChanged();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Point Location
		{
			get { return _bounds.Location; }
			internal set
			{
				if (value == _bounds.Location)
				{
					return;
				}

				_bounds.Location = value;

				_locationInParent = Parent != null ? _bounds.Location - Parent.Location : _bounds.Location;

				FireLocationChanged();
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
				Size = value.Size;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Rectangle ClientBounds
		{
			get
			{
				var clientBounds = Bounds;

				clientBounds.X += _padding.Left;
				clientBounds.Y += _padding.Top;

				clientBounds.Width -= _padding.Width;
				if (clientBounds.Width < 0)
				{
					clientBounds.Width = 0;
				}

				clientBounds.Height -= _padding.Height;
				if (clientBounds.Height < 0)
				{
					clientBounds.Height = 0;
				}

				return clientBounds;
			}

		}

		[HiddenInEditor]
		[JsonIgnore]
		public virtual bool CanFocus
		{
			get { return false; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public virtual bool AcceptsTab
		{
			get { return false; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public bool IsFocused { get; internal set; }

		public event EventHandler VisibleChanged;
		public event EventHandler MeasureChanged;
		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;

		public event EventHandler MouseLeft;
		public event EventHandler<GenericEventArgs<Point>> MouseEntered;
		public event EventHandler<GenericEventArgs<Point>> MouseMoved;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseDown;
		public event EventHandler<GenericEventArgs<MouseButtons>> MouseUp;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<char>> KeyPressed;
		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;

		public Widget()
		{
			InvalidateLayout();

			Visible = true;
			Enabled = true;
		}

		public virtual Drawable GetCurrentBackground()
		{
			var result = Background;
			if (!Enabled && DisabledBackground != null)
			{
				result = DisabledBackground;
			}
			else if (IsMouseOver && OverBackground != null)
			{
				result = OverBackground;
			}

			return result;
		}

		public virtual void Render(SpriteBatch batch)
		{
			var bounds = Bounds;

			if (bounds.IsEmpty)
			{
				return;
			}

			var oldScissorRectangle = batch.GraphicsDevice.ScissorRectangle;
			var newScissorRectangle = Rectangle.Intersect(oldScissorRectangle, Bounds);

			if (newScissorRectangle.IsEmpty)
			{
				return;
			}

			batch.FlushUI();
			batch.GraphicsDevice.ScissorRectangle = newScissorRectangle;

			UpdateLayout();

			// Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				background.Draw(batch, bounds);
			}

			InternalRender(batch);

			if (DrawFrames)
			{
				batch.DrawRect(Color.LightGreen, bounds);
			}

			if (DrawFocused && IsFocused)
			{
				batch.DrawRect(Color.Red, bounds);
			}

			batch.FlushUI();
			batch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
		}

		public virtual void InternalRender(SpriteBatch batch)
		{
		}

		public Point Measure(Point availableSize)
		{
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

			result.X += _padding.Width;
			result.Y += _padding.Height;

			return result;
		}

		protected virtual Point InternalMeasure(Point availableSize)
		{
			return Point.Zero;
		}

		public virtual void Arrange()
		{
		}

		public void InvalidateLayout()
		{
			LayoutInvalid = true;
		}

		public virtual void UpdateLayout()
		{
			if (!LayoutInvalid)
			{
				return;
			}

			Arrange();

			LayoutInvalid = false;
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
				foreach (var widget in asContainer.Items)
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

		public virtual void FireLocationChanged()
		{
			var ev = LocationChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public void FireSizeChanged()
		{
			InvalidateLayout();

			var ev = SizeChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public virtual void FireMeasureChanged()
		{
			InvalidateLayout();

			var ev = MeasureChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public void ApplyWidgetStyle(WidgetStyle style)
		{
			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			Padding = style.Padding;
		}

		private void FrameInfoOnSizeChanged(object sender, EventArgs eventArgs)
		{
			FireMeasureChanged();
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
	}
}