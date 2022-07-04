using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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
	public struct MouseInfo
	{
		public Point Position;
		public bool IsLeftButtonDown;
		public bool IsMiddleButtonDown;
		public bool IsRightButtonDown;
		public float Wheel;
	}

	public class Desktop
	{
		public const int DoubleClickIntervalInMs = 500;
		public const int DoubleClickRadius = 2;

		private Rectangle _bounds;
		private Vector2 _scale = Vector2.One;
		private Vector2 _transformOrigin = new Vector2(0.5f, 0.5f);
		private float _rotation = 0.0f;
		private Transform? _transform;

		private RenderContext _renderContext;

		private bool _layoutDirty = true;
		private bool _widgetsDirty = true;
		private Widget _focusedKeyboardWidget, _mouseInsideWidget;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		private DateTime _lastTouchDown;
		private DateTime? _lastKeyDown;
		private int _keyDownCount = 0;
		private MouseInfo _lastMouseInfo;
		private readonly bool[] _downKeys = new bool[0xff], _lastDownKeys = new bool[0xff];
		private Widget _previousKeyboardFocus;
#if MONOGAME || FNA || PLATFORM_AGNOSTIC
		private TouchCollection _oldTouchState;
#endif
		private bool _isTouchDown;
		private Point _previousMousePosition, _mousePosition, _previousTouchPosition, _touchPosition;
		private bool _contextMenuShown = false;
		private bool _keyboardFocusSet = false;
#if MONOGAME || PLATFORM_AGNOSTIC
		public bool HasExternalTextInput = false;
#endif

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
				Widgets.Clear();

				if (value != null)
				{
					Widgets.Add(value);
				}
			}
		}

		public bool[] DownKeys
		{
			get
			{
				return _downKeys;
			}
		}

		public Point PreviousMousePosition
		{
			get
			{
				return _previousMousePosition;
			}
		}

		public Point MousePosition
		{
			get
			{
				return _mousePosition;
			}

			private set
			{
				if (value == _mousePosition)
				{
					return;
				}

				_previousMousePosition = _mousePosition;
				_mousePosition = value;
				MouseMoved.Invoke();

				ChildrenCopy.ProcessMouseMovement();

				if (IsTouchDown)
				{
					TouchPosition = MousePosition;
				}
			}
		}

		public Point TouchPosition
		{
			get
			{
				return _touchPosition;
			}

			private set
			{
				_previousTouchPosition = _touchPosition;

				if (value == _touchPosition)
				{
					return;
				}

				_touchPosition = value;
				TouchMoved.Invoke();

				ChildrenCopy.ProcessTouchMovement();
			}
		}

		public HorizontalMenu MenuBar { get; set; }

		public Func<MouseInfo> MouseInfoGetter
		{
			get; set;
		}

		public Action<bool[]> DownKeysGetter
		{
			get; set;
		}

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

		/// <summary>
		/// Widget having keyboard focus
		/// </summary>
		public Widget FocusedKeyboardWidget
		{
			get { return _focusedKeyboardWidget; }

			set
			{
				if (value != null)
				{
					_keyboardFocusSet = true;
				}

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

		public Widget MouseInsideWidget
		{
			get => _mouseInsideWidget;
			set
			{
				if (value == _mouseInsideWidget)
				{
					return;
				}

				_mouseInsideWidget = value;
				MouseInsideWidgetChanged.Invoke(this);
			}
		}

		private RenderContext RenderContext
		{
			get
			{
				EnsureRenderContext();

				return _renderContext;
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
				return IsPointOverGUI(TouchPosition);
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

		public bool IsTouchDown
		{
			get
			{
				return _isTouchDown;
			}

			set
			{
				if (value == _isTouchDown)
				{
					return;
				}

				_isTouchDown = value;
				if (_isTouchDown)
				{

					InputOnTouchDown();
					TouchDown.Invoke();
				}
				else
				{
					InputOnTouchUp();
					TouchUp.Invoke();
				}
			}
		}

		public int RepeatKeyDownStartInMs { get; set; } = 500;

		public int RepeatKeyDownInternalInMs { get; set; } = 50;

		public bool HasModalWidget
		{
			get
			{
				for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
				{
					var w = ChildrenCopy[i];
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

		public Action<Keys> KeyDownHandler;

		public event EventHandler MouseMoved;

		public event EventHandler TouchMoved;
		public event EventHandler TouchDown;
		public event EventHandler TouchUp;
		public event EventHandler TouchDoubleClick;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;
		public event EventHandler<GenericEventArgs<char>> Char;

		public event EventHandler<CancellableEventArgs<Widget>> ContextMenuClosing;
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		public event EventHandler<CancellableEventArgs<Widget>> WidgetLosingKeyboardFocus;
		public event EventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		public event EventHandler MouseInsideWidgetChanged;

		public Desktop()
		{
			Opacity = 1.0f;
			Widgets.CollectionChanged += WidgetsOnCollectionChanged;

			MouseInfoGetter = DefaultMouseInfoGetter;
			DownKeysGetter = DefaultDownKeysGetter;

			KeyDownHandler = OnKeyDown;

#if FNA
			TextInputEXT.TextInput += c =>
			{
				OnChar(c);
			};
#endif
		}

		public bool IsKeyDown(Keys keys)
		{
			return _downKeys[(int)keys];
		}

		public MouseInfo DefaultMouseInfoGetter()
		{
#if MONOGAME || FNA
			var state = Mouse.GetState();

			return new MouseInfo
			{
				Position = new Point(state.X, state.Y),
				IsLeftButtonDown = state.LeftButton == ButtonState.Pressed,
				IsMiddleButtonDown = state.MiddleButton == ButtonState.Pressed,
				IsRightButtonDown = state.RightButton == ButtonState.Pressed,
				Wheel = state.ScrollWheelValue
			};
#elif STRIDE
			var input = MyraEnvironment.Game.Input;

			var v = input.AbsoluteMousePosition;

			return new MouseInfo
			{
				Position = new Point((int)v.X, (int)v.Y),
				IsLeftButtonDown = input.IsMouseButtonDown(MouseButton.Left),
				IsMiddleButtonDown = input.IsMouseButtonDown(MouseButton.Middle),
				IsRightButtonDown = input.IsMouseButtonDown(MouseButton.Right),
				Wheel = input.MouseWheelDelta
			};
#else
			return MyraEnvironment.Platform.GetMouseInfo();
#endif
		}

		public void DefaultDownKeysGetter(bool[] keys)
		{
#if MONOGAME || FNA
			var state = Keyboard.GetState();
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = state.IsKeyDown((Keys)i);
			}
#elif STRIDE
			var input = MyraEnvironment.Game.Input;
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = input.IsKeyDown((Keys)i);
			}
#else
			MyraEnvironment.Platform.SetKeysDown(keys);
#endif
		}

		public Widget GetChild(int index)
		{
			return ChildrenCopy[index];
		}

		private void HandleDoubleClick()
		{
			if ((DateTime.Now - _lastTouchDown).TotalMilliseconds < DoubleClickIntervalInMs &&
				Math.Abs(_touchPosition.X - _previousTouchPosition.X) <= DoubleClickRadius &&
				Math.Abs(_touchPosition.Y - _previousTouchPosition.Y) <= DoubleClickRadius)
			{
				TouchDoubleClick.Invoke();

				ChildrenCopy.ProcessTouchDoubleClick();

				_lastTouchDown = DateTime.MinValue;
			}
			else
			{
				_lastTouchDown = DateTime.Now;
			}
		}

		private void ContextMenuOnTouchDown()
		{
			if (ContextMenu == null || ContextMenu.ContainsTouch)
			{
				return;
			}

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

		private void InputOnTouchDown()
		{
			_contextMenuShown = false;
			_keyboardFocusSet = false;

			ChildrenCopy.ProcessTouchDown();

			if (!_keyboardFocusSet && FocusedKeyboardWidget != null)
			{
				// Nullify keyboard focus
				FocusedKeyboardWidget = null;
			}

			if (!_contextMenuShown)
			{
				ContextMenuOnTouchDown();
			}
		}

		private void InputOnTouchUp()
		{
			ChildrenCopy.ProcessTouchUp();
		}

		public void ShowContextMenu(Widget menu, Point position)
		{
			HideContextMenu();

			ContextMenu = menu;
			if (ContextMenu == null)
			{
				return;
			}

			ContextMenu.HorizontalAlignment = HorizontalAlignment.Left;
			ContextMenu.VerticalAlignment = VerticalAlignment.Top;

			var measure = ContextMenu.Measure(LayoutBounds.Size());

			if (position.X + measure.X > LayoutBounds.Right)
			{
				position.X = LayoutBounds.Right - measure.X;
			}

			if (position.Y + measure.Y > LayoutBounds.Bottom)
			{
				position.Y = LayoutBounds.Bottom - measure.Y;
			}

			ContextMenu.Left = position.X;
			ContextMenu.Top = position.Y;

			ContextMenu.Visible = true;

			Widgets.Add(ContextMenu);

			if (ContextMenu.AcceptsKeyboardFocus)
			{
				_previousKeyboardFocus = FocusedKeyboardWidget;
				FocusedKeyboardWidget = ContextMenu;
			}

			_contextMenuShown = true;
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

		private void EnsureRenderContext()
		{
			if (_renderContext == null)
			{
				_renderContext = new RenderContext();
			}
		}

		public void RenderVisual()
		{
			EnsureRenderContext();

			var oldScissorRectangle = _renderContext.Scissor;

			_renderContext.Begin();

			// Disable transform during setting the scissor rectangle for the Desktop
			_renderContext.Transform = Transform;

			var bounds = _renderContext.Transform.Apply(LayoutBounds);
			_renderContext.Scissor = bounds;
			_renderContext.Opacity = Opacity;

			if (Stylesheet.Current.DesktopStyle != null &&
				Stylesheet.Current.DesktopStyle.Background != null)
			{
				Stylesheet.Current.DesktopStyle.Background.Draw(_renderContext, LayoutBounds);
			}

			foreach (var widget in ChildrenCopy)
			{
				if (widget.Visible)
				{
					widget.Render(_renderContext);
				}
			}

			_renderContext.End();

			_renderContext.Scissor = oldScissorRectangle;
		}

		public void Render()
		{
			UpdateInput();
			UpdateLayout();
			RenderVisual();
		}

		private void InvalidateTransform()
		{
			_transform = null;

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
			}
		}

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
			var active = true;
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var w = ChildrenCopy[i];
				if (!w.Visible)
				{
					continue;
				}

				w.ProcessWidgets(widget =>
				{
					widget.Active = active;

					if (MenuBar == null && widget is HorizontalMenu)
					{
						// Found MenuBar
						MenuBar = (HorizontalMenu)widget;
					}

					// Continue
					return true;
				});

				// Everything after first modal widget is not active
				if (w.IsModal)
				{
					active = false;
				}
			}

			UpdateRecursiveLayout(ChildrenCopy);

			// Fire Mouse Movement without actual mouse movement in order to update Widget.IsMouseInside
			_previousMousePosition = _mousePosition;
			ChildrenCopy.ProcessMouseMovement();

			_layoutDirty = false;
		}

		internal void ProcessWidgets(Func<Widget, bool> operation)
		{
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var w = ChildrenCopy[i];
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

				var c = i as Container;
				if (c != null)
				{
					UpdateRecursiveLayout(c.ChildrenCopy);
				}
			}
		}

		private Widget GetWidgetBy(Widget root, Func<Widget, bool> filter)
		{
			if (filter(root))
			{
				return root;
			}

			var asContainer = root as Container;
			if (asContainer == null)
			{
				return null;
			}

			for (var i = 0; i < asContainer.ChildrenCount; ++i)
			{
				var w = asContainer.GetChild(i);
				var result = GetWidgetBy(w, filter);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public Widget GetWidgetBy(Func<Widget, bool> filter)
		{
			foreach (var w in ChildrenCopy)
			{
				var result = GetWidgetBy(w, filter);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public Widget GetWidgetByID(string ID)
		{
			return GetWidgetBy(w => w.Id == ID);
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

				var asContainer = w as Container;
				if (asContainer != null)
				{
					result += asContainer.CalculateTotalChildCount(visibleOnly);
				}
			}

			return result;
		}

		public void HandleButton(bool isDown, bool wasDown, MouseButtons buttons)
		{
			if (isDown && !wasDown)
			{
				TouchPosition = MousePosition;
				IsTouchDown = true;
				HandleDoubleClick();
			}
			else if (!isDown && wasDown)
			{
				IsTouchDown = false;
			}
		}

#if MONOGAME || FNA || PLATFORM_AGNOSTIC
		public void UpdateTouch()
		{
#if MONOGAME || FNA
			var touchState = TouchPanel.GetState();
#else
			var touchState = MyraEnvironment.Platform.GetTouchState();
#endif

			if (!touchState.IsConnected)
			{
				return;
			}

			if (touchState.Count > 0)
			{
				var pos = touchState[0].Position;
				TouchPosition = new Point((int)pos.X, (int)pos.Y);
			}

			if (touchState.Count > 0 && _oldTouchState.Count == 0)
			{
				// Down
				IsTouchDown = true;
				HandleDoubleClick();
			}
			else if (touchState.Count == 0 && _oldTouchState.Count > 0)
			{
				// Up
				IsTouchDown = false;
			}

			_oldTouchState = touchState;
		}
#endif

		public void UpdateMouseInput()
		{
			if (MouseInfoGetter == null)
			{
				return;
			}

			var mouseInfo = MouseInfoGetter();
			var mousePosition = mouseInfo.Position;

			EnsureRenderContext();
			MousePosition = mousePosition;

			HandleButton(mouseInfo.IsLeftButtonDown, _lastMouseInfo.IsLeftButtonDown, MouseButtons.Left);
			HandleButton(mouseInfo.IsMiddleButtonDown, _lastMouseInfo.IsMiddleButtonDown, MouseButtons.Middle);
			HandleButton(mouseInfo.IsRightButtonDown, _lastMouseInfo.IsRightButtonDown, MouseButtons.Right);
#if STRIDE
				var handleWheel = mouseInfo.Wheel != 0;
#else
			var handleWheel = mouseInfo.Wheel != _lastMouseInfo.Wheel;
#endif

			if (handleWheel)
			{
				var delta = mouseInfo.Wheel;
#if !STRIDE
				delta -= _lastMouseInfo.Wheel;
#endif
				MouseWheelChanged.Invoke(delta);

				Widget mouseWheelFocusedWidget = null;
				if (FocusedKeyboardWidget != null && FocusedKeyboardWidget.MouseWheelFocusType == MouseWheelFocusType.Focus)
				{
					mouseWheelFocusedWidget = FocusedKeyboardWidget;
				}
				else
				{
					// Go through the parents chain in order to find first widget that accepts mouse wheel events
					var widget = MouseInsideWidget;
					while (widget != null)
					{
						if (widget.MouseWheelFocusType == MouseWheelFocusType.Hover)
						{
							mouseWheelFocusedWidget = widget;
							break;
						}

						widget = widget.Parent;
					}
				}

				if (mouseWheelFocusedWidget != null)
				{
					mouseWheelFocusedWidget.OnMouseWheel(delta);
				}

			}

			_lastMouseInfo = mouseInfo;
		}

		public void UpdateKeyboardInput()
		{
			if (DownKeysGetter == null)
			{
				return;
			}

			DownKeysGetter(_downKeys);

			var now = DateTime.Now;
			for(var i = 0; i < _downKeys.Length; ++i)
			{
				var key = (Keys)i;
				if (_downKeys[i] && !_lastDownKeys[i])
				{
					if (key == Keys.Tab)
					{
						FocusNextWidget();
					}

					KeyDownHandler?.Invoke(key);

					_lastKeyDown = now;
					_keyDownCount = 0;
				} else if (!_downKeys[i] && _lastDownKeys[i])
				{
					// Key had been released
					KeyUp.Invoke(key);
					if (_focusedKeyboardWidget != null && _focusedKeyboardWidget.Active)
					{
						_focusedKeyboardWidget.OnKeyUp(key);
					}

					_lastKeyDown = null;
					_keyDownCount = 0;
				} else if (_downKeys[i] && _lastDownKeys[i])
				{
					if (_lastKeyDown != null &&
									  ((_keyDownCount == 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownStartInMs) ||
									  (_keyDownCount > 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownInternalInMs)))
					{
						KeyDownHandler?.Invoke(key);

						_lastKeyDown = now;
						++_keyDownCount;
					}
				}
			}

			Array.Copy(_downKeys, _lastDownKeys, _downKeys.Length);
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
			widget != null && widget.Visible && widget.Active &&
			widget.Enabled && widget.AcceptsKeyboardFocus;

		public void UpdateInput()
		{
			UpdateMouseInput();
			UpdateKeyboardInput();

#if MONOGAME || FNA
			try
			{
				UpdateTouch();
			}
			catch (Exception)
			{
			}
#endif
		}

		public void OnKeyDown(Keys key)
		{
			KeyDown.Invoke(key);

			if (IsMenuBarActive)
			{
				MenuBar.OnKeyDown(key);
			}
			else
			{
				if (_focusedKeyboardWidget != null && _focusedKeyboardWidget.Active)
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

			if (_focusedKeyboardWidget != null && _focusedKeyboardWidget.Active)
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

		private bool InternalIsPointOverGUI(Point p, Widget w)
		{
			if (!w.Visible || !w.ContainsGlobalPoint(p))
			{
				return false;
			}

			if (!w.FallsThrough(p))
			{
				return true;
			}

			// If widget fell through, then it is Container for sure
			var asContainer = (Container)w;

			// Or if any child is solid
			foreach (var ch in asContainer.ChildrenCopy)
			{
				if (InternalIsPointOverGUI(p, ch))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsPointOverGUI(Point p)
		{
			foreach (var widget in ChildrenCopy)
			{
				if (InternalIsPointOverGUI(p, widget))
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
	}
}