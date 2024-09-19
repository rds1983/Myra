using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
using System.Numerics;
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D.UI
{
	public partial class Desktop : ITransformable, IDisposable
	{
		private Rectangle _bounds;
		private Vector2 _scale = Vector2.One;
		private Vector2 _transformOrigin = Vector2.Zero;
		private float _rotation = 0.0f;
		private Transform? _transform;
		private Matrix _inverseMatrix;
		private bool _inverseMatrixDirty = true;

		private readonly InputContext _inputContext = new InputContext();
		private readonly RenderContext _renderContext = new RenderContext();

		private bool _layoutDirty = true;
		private bool _widgetsDirty = true;
		private Widget _focusedKeyboardWidget;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		private Widget _previousKeyboardFocus;
#if MONOGAME || PLATFORM_AGNOSTIC
		public bool HasExternalTextInput = false;
#endif

		private bool _isDisposed = false;

		/// <summary>
		/// Root Widget
		/// </summary>
		public Widget Root
		{
			get
			{
				if (Widgets.Count == 0)
				{
					return null;
				}

				return Widgets[0];
			}

			set
			{
				if (Root == value)
				{
					return;
				}

				HideContextMenu();
				HideTooltip();
				Widgets.Clear();

				if (value != null)
				{
					Widgets.Add(value);
				}
			}
		}

		public HorizontalMenu MenuBar { get; private set; }

		internal List<Widget> ChildrenCopy
		{
			get
			{
				UpdateWidgetsCopy();
				return _widgetsCopy;
			}
		}

		public ObservableCollection<Widget> Widgets { get; } = new ObservableCollection<Widget>();

		public Func<Rectangle> BoundsFetcher = DefaultBoundsFetcher;

		internal Rectangle InternalBounds
		{
			get => _bounds;

			set
			{
				if (_bounds == value)
				{
					return;
				}

				_bounds = value;


				InvalidateTransform();
			}
		}

		internal Rectangle LayoutBounds => new Rectangle(0, 0, InternalBounds.Width, InternalBounds.Height);

		public Widget ContextMenu { get; private set; }
		public Widget Tooltip { get; private set; }

		/// <summary>
		/// Widget having keyboard focus
		/// </summary>
		public Widget FocusedKeyboardWidget
		{
			get { return _focusedKeyboardWidget; }

			set
			{
				if (value == _focusedKeyboardWidget)
				{
					return;
				}

				var oldValue = _focusedKeyboardWidget;
				if (oldValue != null)
				{
					if (WidgetLosingKeyboardFocus != null)
					{
						var args = new CancellableEventArgs<Widget>(oldValue);
						WidgetLosingKeyboardFocus(null, args);
						if (oldValue.IsPlaced && args.Cancel)
						{
							return;
						}
					}
				}

				_focusedKeyboardWidget = value;
				if (oldValue != null)
				{
					oldValue.OnLostKeyboardFocus();
				}

				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnGotKeyboardFocus();
					WidgetGotKeyboardFocus.Invoke(_focusedKeyboardWidget);
				}
			}
		}

		public float Opacity { get; set; }

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

		internal Transform Transform
		{
			get
			{
				if (_transform == null)
				{
					_transform = new Transform(_bounds.Location.ToVector2(),
						TransformOrigin * _bounds.Size().ToVector2(),
						Scale,
						Rotation * (float)Math.PI / 180);
				}

				return _transform.Value;
			}
		}

		public bool IsMouseOverGUI
		{
			get
			{
				return IsPointOverGUI(MousePosition);
			}
		}

		public bool IsTouchOverGUI
		{
			get
			{
				return TouchPosition != null && IsPointOverGUI(TouchPosition.Value);
			}
		}

		internal bool IsShiftDown
		{
			get
			{
				return IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
			}
		}

		internal bool IsControlDown
		{
			get
			{
#if !STRIDE
				return IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
#else
				return IsKeyDown(Keys.LeftCtrl) || IsKeyDown(Keys.RightCtrl);
#endif
			}
		}

		internal bool IsAltDown
		{
			get
			{
#if !STRIDE
				return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
#else
				return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
#endif
			}
		}

		public bool HasModalWidget
		{
			get
			{
				var childrenCopy = ChildrenCopy;
				for (var i = childrenCopy.Count - 1; i >= 0; --i)
				{
					var w = childrenCopy[i];
					if (w.Visible && w.Enabled && w.IsModal)
					{
						return true;
					}
				}

				return false;
			}
		}

		private bool IsMenuBarActive
		{
			get
			{
				return MenuBar != null && (MenuBar.OpenMenuItem != null || IsAltDown);
			}
		}

		public IBrush Background { get; set; }

		public event EventHandler<CancellableEventArgs<Widget>> ContextMenuClosing;
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		public event EventHandler<CancellableEventArgs<Widget>> WidgetLosingKeyboardFocus;
		public event EventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		public Action<Keys> KeyDownHandler;

		public Desktop()
		{
			Opacity = 1.0f;
			Widgets.CollectionChanged += WidgetsOnCollectionChanged;
			KeyDownHandler = OnKeyDown;

#if FNA
			TextInputEXT.TextInput += OnChar;
#endif

			if (Stylesheet.Current.DesktopStyle != null)
			{
				Background = Stylesheet.Current.DesktopStyle.Background;
			}
		}

		public bool IsKeyDown(Keys keys)
		{
			return _downKeys[(int)keys];
		}

		public Widget GetChild(int index)
		{
			return ChildrenCopy[index];
		}

		private void InputOnTouchDown()
		{
			if (ContextMenu != null && !ContextMenu.IsTouchInside)
			{
				var ev = ContextMenuClosing;
				if (ev != null)
				{
					var args = new CancellableEventArgs<Widget>(ContextMenu);
					ev(null, args);

					if (args.Cancel)
					{
						return;
					}
				}
				HideContextMenu();
			}

			HideTooltip();
		}

		public void HideContextMenu()
		{
			if (ContextMenu == null)
			{
				return;
			}

			Widgets.Remove(ContextMenu);
			ContextMenu.Visible = false;

			ContextMenuClosed.Invoke(ContextMenu);
			ContextMenu = null;

			if (_previousKeyboardFocus != null)
			{
				FocusedKeyboardWidget = _previousKeyboardFocus;
				_previousKeyboardFocus = null;
			}
		}

		private void FixOverWidgetPosition(Widget widget, Point position)
		{
			widget.HorizontalAlignment = HorizontalAlignment.Left;
			widget.VerticalAlignment = VerticalAlignment.Top;

			var measure = widget.Measure(LayoutBounds.Size());

			if (position.X + measure.X > LayoutBounds.Right)
			{
				position.X = LayoutBounds.Right - measure.X;
			}

			if (position.Y + measure.Y > LayoutBounds.Bottom)
			{
				position.Y = LayoutBounds.Bottom - measure.Y;
			}

			widget.Left = position.X;
			widget.Top = position.Y;
		}

		public void ShowContextMenu(Widget menu, Point position)
		{
			HideContextMenu();

			ContextMenu = menu;
			if (ContextMenu == null)
			{
				return;
			}


			FixOverWidgetPosition(menu, position);

			ContextMenu.Visible = true;
			Widgets.Add(ContextMenu);

			if (ContextMenu.AcceptsKeyboardFocus)
			{
				_previousKeyboardFocus = FocusedKeyboardWidget;
				FocusedKeyboardWidget = ContextMenu;
			}
		}

		public void HideTooltip()
		{
			if (Tooltip == null)
			{
				return;
			}

			Widgets.Remove(Tooltip);
			Tooltip.Visible = false;
			Tooltip = null;
		}

		public void ShowTooltip(Widget widget, Point position)
		{
			if (string.IsNullOrEmpty(widget.Tooltip))
			{
				return;
			}

			HideTooltip();
			Tooltip = MyraEnvironment.TooltipCreator(widget);
			if (Tooltip == null)
			{
				return;
			}

			FixOverWidgetPosition(Tooltip, position);

			Tooltip.Visible = true;
			Widgets.Add(Tooltip);
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					w.Desktop = this;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					w.Desktop = null;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (Widget w in ChildrenCopy)
				{
					w.Desktop = null;
				}
			}

			InvalidateLayout();
			_widgetsDirty = true;
		}

		public void RenderVisual()
		{
			var oldDeviceScissor = _renderContext.DeviceScissor;

			_renderContext.Begin();

			// Disable transform during setting the scissor rectangle for the Desktop
			_renderContext.Transform = Transform;

			var bounds = _renderContext.Transform.Apply(LayoutBounds);
			_renderContext.Scissor = bounds;
			_renderContext.Opacity = Opacity;

			if (Background != null)
			{
				Background.Draw(_renderContext, LayoutBounds);
			}

			foreach (var widget in ChildrenCopy)
			{
				if (widget.Visible)
				{

					if (MyraEnvironment.EnableModalDarkening && widget.IsModal)
					{
						_renderContext.FillRectangle(bounds, MyraEnvironment.DarkeningColor);
					}

					widget.Render(_renderContext);
				}
			}

			_renderContext.End();

			_renderContext.DeviceScissor = oldDeviceScissor;
		}

		public void Render()
		{
			// Layout run
			UpdateLayout();

			// First input run: set Desktop/Widgets input states and schedule input events
			UpdateInput();

			_inputContext.Reset();

			var childrenCopy = ChildrenCopy;
			for (var i = childrenCopy.Count - 1; i >= 0; --i)
			{
				var widget = childrenCopy[i];
				widget.ProcessInput(_inputContext);
			}

			// Only one widget at a time can receive mouse wheel event
			// So scheduling it here
			if (_inputContext.MouseWheelWidget != null)
			{
				InputEventsManager.Queue(_inputContext.MouseWheelWidget, InputEventType.MouseWheel);
			}

			// Second input run: process input events
			InputEventsManager.ProcessEvents();

			// Do another layout run, since an input event could cause the layout change
			UpdateLayout();

			// Render run
			RenderVisual();
		}

		private void InvalidateTransform()
		{
			_transform = null;
			_inverseMatrixDirty = true;

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
			}
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

		public void InvalidateLayout()
		{
			_layoutDirty = true;
		}

		public void UpdateLayout()
		{
			var newBounds = BoundsFetcher();
			if (InternalBounds != newBounds)
			{
				InvalidateLayout();
			}

			InternalBounds = newBounds;

			if (InternalBounds.IsEmpty)
			{
				return;
			}

			if (!_layoutDirty)
			{
				return;
			}

			foreach (var child in ChildrenCopy)
			{
				if (child.Visible)
				{
					child.Arrange(LayoutBounds);
				}
			}

			// Rest processing
			MenuBar = null;

			var childrenCopy = ChildrenCopy;
			for (var i = childrenCopy.Count - 1; i >= 0; --i)
			{
				var w = childrenCopy[i];
				if (!w.Visible)
				{
					continue;
				}

				MenuBar = w.FindChild<HorizontalMenu>();
				if (MenuBar != null)
				{
					break;
				}
			}

			UpdateRecursiveLayout(ChildrenCopy);

			_layoutDirty = false;
		}

		internal void ProcessWidgets(Func<Widget, bool> operation)
		{
			foreach (var w in ChildrenCopy)
			{
				var result = w.ProcessWidgets(operation);
				if (!result)
				{
					return;
				}
			}
		}

		private void UpdateRecursiveLayout(IEnumerable<Widget> widgets)
		{
			foreach (var i in widgets)
			{
				if (!i.Layout2d.Nullable)
				{
					ExpressionParser.Parse(i, ChildrenCopy);
				}

				UpdateRecursiveLayout(i.ChildrenCopy);
			}
		}

		private Widget FindChild(Widget root, Func<Widget, bool> predicate)
		{
			if (predicate(root))
			{
				return root;
			}

			foreach (var w in root.ChildrenCopy)
			{
				var result = w.FindChild(predicate);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public Widget FindChild(Func<Widget, bool> filter)
		{
			foreach (var w in ChildrenCopy)
			{
				var result = FindChild(w, filter);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		[Obsolete("Use FindChild")]
		public Widget GetWidgetBy(Func<Widget, bool> predicate) => FindChild(predicate);

		public Widget FindChild(string id)
		{
			return FindChild(w => w.Id == id);
		}

		[Obsolete("Use FindChild")]
		public Widget GetWidgetByID(string ID)
		{
			return FindChild(w => w.Id == ID);
		}

		public int CalculateTotalWidgets(bool visibleOnly)
		{
			var result = 0;
			foreach (var w in Widgets)
			{
				if (visibleOnly && !w.Visible)
				{
					continue;
				}

				++result;

				result += w.CalculateTotalChildCount(visibleOnly);
			}

			return result;
		}

		private void FocusNextWidget()
		{
			if (Widgets.Count == 0) return;

			var isNull = FocusedKeyboardWidget == null;
			var focusChanged = false;
			ProcessWidgets(w =>
			{
				if (isNull)
				{
					if (CanFocusWidget(w))
					{
						w.SetKeyboardFocus();
						focusChanged = true;
						return false;
					}
				}
				else
				{
					if (w == FocusedKeyboardWidget)
					{
						isNull = true;
						// Next widget will be focused
					}
				}

				return true;
			});

			if (focusChanged || FocusedKeyboardWidget == null)
			{
				// Either new focus had been set or there are no focusable widgets
				return;
			}

			// Next run - try to focus first widget before focused one
			ProcessWidgets(w =>
			{
				if (CanFocusWidget(w))
				{
					w.SetKeyboardFocus();
					return false;
				}

				return true;
			});
		}

		private static bool CanFocusWidget(Widget widget) =>
			widget != null && widget.Visible &&
			widget.Enabled && widget.AcceptsKeyboardFocus;

		public void OnKeyDown(Keys key)
		{
			KeyDown.Invoke(key);

			if (IsMenuBarActive)
			{
				MenuBar.OnKeyDown(key);
			}
			else
			{
				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnKeyDown(key);

#if STRIDE
					var ch = key.ToChar(IsKeyDown(Keys.LeftShift) ||
										IsKeyDown(Keys.RightShift));
					if (ch != null)
					{
						_focusedKeyboardWidget.OnChar(ch.Value);
					}
#elif MONOGAME || PLATFORM_AGNOSTIC
					if (!HasExternalTextInput && !IsControlDown && !IsAltDown)
					{
						var c = key.ToChar(IsShiftDown);
						if (c != null)
						{
							OnChar(c.Value);
						}
					}
#endif
				}
			}

			if (key == Keys.Escape && ContextMenu != null)
			{
				HideContextMenu();
			}
		}

		public void OnChar(char c)
		{
			if (IsMenuBarActive)
			{
				// Don't accept chars if menubar is open
				return;
			}

			if (_focusedKeyboardWidget != null)
			{
				_focusedKeyboardWidget.OnChar(c);
			}

			Char.Invoke(c);
		}

		private void UpdateWidgetsCopy()
		{
			if (!_widgetsDirty)
			{
				return;
			}

			_widgetsCopy.Clear();
			_widgetsCopy.AddRange(Widgets);

			_widgetsCopy.SortWidgetsByZIndex();

			_widgetsDirty = false;
		}

		public bool IsPointOverGUI(Point p)
		{
			foreach (var widget in ChildrenCopy)
			{
				var result = widget.HitTest(p);
				if (result != null)
				{
					return true;
				}
			}

			return false;
		}

		public static Rectangle DefaultBoundsFetcher()
		{
			var size = CrossEngineStuff.ViewSize;
			return new Rectangle(0, 0, size.X, size.Y);
		}

		private void ReleaseUnmanagedResources()
		{
			_renderContext.Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

#if FNA
			TextInputEXT.TextInput -= OnChar;
#endif

			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~Desktop()
		{
			ReleaseUnmanagedResources();
		}
	}
}