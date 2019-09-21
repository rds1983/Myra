using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Reflection;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Input;
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

		public static Func<Rectangle> DefaultBoundsFetcher = () =>
		{
			var device = MyraEnvironment.GraphicsDevice;
#if !XENKO
			return new Rectangle(0, 0,
				device.PresentationParameters.BackBufferWidth,
				device.PresentationParameters.BackBufferHeight);
#else
			return new Rectangle(0, 0,
				device.Presenter.BackBuffer.ViewWidth, 
				device.Presenter.BackBuffer.ViewHeight);
#endif
		};

		private RenderContext _renderContext;

		private bool _layoutDirty = true;
		private Rectangle _bounds;
		private bool _widgetsDirty = true;
		private Widget _focusedKeyboardWidget;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();
		private DateTime _lastTouchDown;
		private MouseInfo _lastMouseInfo;
		private IReadOnlyCollection<Keys> _downKeys, _lastDownKeys;
		private Widget _previousKeyboardFocus;
		private Widget _previousMouseWheelFocus;
#if !XENKO
		private TouchCollection _oldTouchState;
#endif
		private Widget _scheduleMouseWheelFocus;
		private bool _isTouchDown;
		private Point _mousePosition, _touchPosition;
		private Point _lastMousePosition, _lastTouchPosition;
#if MONOGAME
		public bool HasExternalTextInput = false;
#endif

		public IReadOnlyCollection<Keys> DownKeys
		{
			get
			{
				return _downKeys;
			}
		}

		internal Point LastMousePosition
		{
			get
			{
				return _lastMousePosition;
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

				_lastMousePosition = _mousePosition;
				_mousePosition = value;
				MouseMoved.Invoke(this);

				for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
				{
					var w = ChildrenCopy[i];
					if (w.Visible && w.Enabled)
					{
						var asWindow = w as Window;
						if (w.HandleMouseMovement() || (asWindow != null && asWindow.IsModal))
						{
							break;
						}
					}
				}

				if (IsTouchDown)
				{
					TouchPosition = MousePosition;
				}
			}
		}

		internal Point LastTouchPosition
		{
			get
			{
				return _lastTouchPosition;
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
				if (value == _touchPosition)
				{
					return;
				}

				_lastTouchPosition = _touchPosition;
				_touchPosition = value;
				TouchMoved.Invoke(this);

				for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
				{
					var w = ChildrenCopy[i];
					if (w.Visible && w.Enabled)
					{
						var asWindow = w as Window;
						if (w.HandleTouchMovement() || (asWindow != null && asWindow.IsModal))
						{
							break;
						}
					}
				}
			}
		}

		public HorizontalMenu MenuBar { get; set; }

		public Func<MouseInfo> MouseInfoGetter
		{
			get; set;
		}

		public Func<IReadOnlyCollection<Keys>> DownKeysGetter
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

		public ObservableCollection<Widget> Widgets
		{
			get { return _widgets; }
		}

		public Func<Rectangle> BoundsFetcher = DefaultBoundsFetcher;

		internal Rectangle InternalBounds
		{
			get
			{
				return _bounds;
			}
		}

		public Widget ContextMenu { get; private set; }

		public Widget FocusedKeyboardWidget
		{
			get { return _focusedKeyboardWidget; }

			set
			{
				if (value == _focusedKeyboardWidget)
				{
					return;
				}

				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnLostKeyboardFocus();
					WidgetLostKeyboardFocus?.Invoke(this, new GenericEventArgs<Widget>(_focusedKeyboardWidget));
				}

				_focusedKeyboardWidget = value;

				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnGotKeyboardFocus();
					WidgetGotKeyboardFocus?.Invoke(this, new GenericEventArgs<Widget>(_focusedKeyboardWidget));
				}
			}
		}

		public Widget FocusedMouseWheelWidget
		{
			get; set;
		}

		private RenderContext RenderContext
		{
			get
			{
				EnsureRenderContext();

				return _renderContext;
			}
		}

		/// <summary>
		/// Parameters passed to SpriteBatch.Begin
		/// </summary>
		public SpriteBatchBeginParams SpriteBatchBeginParams
		{
			get
			{
				return RenderContext.SpriteBatchBeginParams;
			}

			set
			{
				RenderContext.SpriteBatchBeginParams = value;
			}
		}

		public float Opacity { get; set; }

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
				return _downKeys.Contains(Keys.LeftShift) || _downKeys.Contains(Keys.RightShift);
			}
		}

		internal bool IsControlDown
		{
			get
			{
#if !XENKO
				return _downKeys.Contains(Keys.LeftControl) || _downKeys.Contains(Keys.RightControl);
#else
				return _downKeys.Contains(Keys.LeftCtrl) || _downKeys.Contains(Keys.RightCtrl);
#endif
			}
		}

		internal bool IsAltDown
		{
			get
			{
#if !XENKO
				return _downKeys.Contains(Keys.LeftAlt) || _downKeys.Contains(Keys.RightAlt);
#else
				return _downKeys.Contains(Keys.LeftAlt) || _downKeys.Contains(Keys.RightAlt);
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
				var activeWidget = GetActiveWidget();
				if (_isTouchDown)
				{
					InputOnTouchDown();

					TouchDown.Invoke(this);
					if (activeWidget != null)
					{
						activeWidget.HandleTouchDown();
					}
				}
				else
				{
					TouchUp.Invoke(this);
					if (activeWidget != null)
					{
						activeWidget.HandleTouchUp();
					}
				}
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

		public event EventHandler<ContextMenuClosingEventArgs> ContextMenuClosing;
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		public event EventHandler<GenericEventArgs<Widget>> WidgetLostKeyboardFocus;
		public event EventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		public Desktop()
		{
			Opacity = 1.0f;
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

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

#if !XENKO
		public static MouseInfo DefaultMouseInfoGetter()
		{
			var state = Mouse.GetState();

			var result = new MouseInfo
			{
				Position = new Point(state.X, state.Y),
				IsLeftButtonDown = state.LeftButton == ButtonState.Pressed,
				IsMiddleButtonDown = state.MiddleButton == ButtonState.Pressed,
				IsRightButtonDown = state.RightButton == ButtonState.Pressed,
				Wheel = state.ScrollWheelValue
			};

			return result;
		}

		public static IReadOnlyCollection<Keys> DefaultDownKeysGetter()
		{
			return Keyboard.GetState().GetPressedKeys();
		}
#else
		public static MouseInfo DefaultMouseInfoGetter()
		{
			var input = MyraEnvironment.Game.Input;

			var v = input.AbsoluteMousePosition;

			var result = new MouseInfo
			{
				Position = new Point((int)v.X, (int)v.Y),
				IsLeftButtonDown = input.IsMouseButtonDown(MouseButton.Left),
				IsMiddleButtonDown = input.IsMouseButtonDown(MouseButton.Middle),
				IsRightButtonDown = input.IsMouseButtonDown(MouseButton.Right),
				Wheel = input.MouseWheelDelta
			};

			return result;
		}

		public static IReadOnlyCollection<Keys> DefaultDownKeysGetter()
		{
			var input = MyraEnvironment.Game.Input;

			return input.Keyboard.DownKeys;
		}
#endif

		public Widget GetChild(int index)
		{
			return ChildrenCopy[index];
		}

		private void HandleDoubleClick()
		{
			if ((DateTime.Now - _lastTouchDown).TotalMilliseconds < DoubleClickIntervalInMs)
			{
				TouchDoubleClick.Invoke(this);

				var activeWidget = GetActiveWidget();
				if (activeWidget != null)
				{
					activeWidget.HandleTouchDoubleClick();
				}

				_lastTouchDown = DateTime.MinValue;
			}
			else
			{
				_lastTouchDown = DateTime.Now;
			}
		}

		private void InputOnTouchDown()
		{
			// Handle context menu
			if (ContextMenu != null && !ContextMenu.Bounds.Contains(TouchPosition))
			{
				var ev = ContextMenuClosing;
				if (ev != null)
				{
					var args = new ContextMenuClosingEventArgs(ContextMenu);
					ev(this, args);

					if (args.Cancel)
					{
						return;
					}
				}

				HideContextMenu();
			}

			// Handle focus
			var activeWidget = GetActiveWidget();
			if (activeWidget == null)
			{
				return;
			}

			// Widgets at the bottom of tree become focused
			Widget focusedWidget = null;
			ProcessWidgets(activeWidget, s =>
			{
				if (s.Enabled && s.IsTouchOver && s.AcceptsKeyboardFocus)
				{
					focusedWidget = s;
				}
			});
			FocusedKeyboardWidget = focusedWidget;

			focusedWidget = null;
			ProcessWidgets(activeWidget, s =>
			{
				if (s.Enabled && s.IsTouchOver && s.AcceptsMouseWheelFocus)
				{
					focusedWidget = s;
				}
			});

			if (focusedWidget != null)
			{
				FocusedMouseWheelWidget = focusedWidget;
			}
		}

		public void ShowContextMenu(Widget menu, Point position)
		{
			if (menu == null)
			{
				throw new ArgumentNullException("menu");
			}

			HideContextMenu();

			ContextMenu = menu;

			if (ContextMenu != null)
			{
				ContextMenu.HorizontalAlignment = HorizontalAlignment.Left;
				ContextMenu.VerticalAlignment = VerticalAlignment.Top;

				var measure = ContextMenu.Measure(InternalBounds.Size());

				if (position.X + measure.X > InternalBounds.Right)
				{
					position.X = InternalBounds.Right - measure.X;
				}

				if (position.Y + measure.Y > InternalBounds.Bottom)
				{
					position.Y = InternalBounds.Bottom - measure.Y;
				}

				ContextMenu.Left = position.X;
				ContextMenu.Top = position.Y;

				ContextMenu.Visible = true;

				_widgets.Add(ContextMenu);

				if (ContextMenu.AcceptsKeyboardFocus)
				{
					_previousKeyboardFocus = FocusedKeyboardWidget;
					FocusedKeyboardWidget = ContextMenu;
				}

				_scheduleMouseWheelFocus = ContextMenu;
			}
		}

		public void HideContextMenu()
		{
			if (ContextMenu == null)
			{
				return;
			}

			_widgets.Remove(ContextMenu);
			ContextMenu.Visible = false;

			ContextMenuClosed.Invoke(this, ContextMenu);
			ContextMenu = null;

			if (_previousKeyboardFocus != null)
			{
				FocusedKeyboardWidget = _previousKeyboardFocus;
				_previousKeyboardFocus = null;
			}

			if (_previousMouseWheelFocus != null)
			{
				FocusedMouseWheelWidget = _previousMouseWheelFocus;
				_previousMouseWheelFocus = null;
			}
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					w.Desktop = this;
					w.MeasureChanged += WOnMeasureChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					w.MeasureChanged -= WOnMeasureChanged;
					w.Desktop = null;
				}
			}

			InvalidateLayout();

			_widgetsDirty = true;
		}

		private void WOnMeasureChanged(object sender, EventArgs eventArgs)
		{
			InvalidateLayout();
		}

		private void EnsureRenderContext()
		{
			if (_renderContext == null)
			{
				var spriteBatch = new SpriteBatch(MyraEnvironment.GraphicsDevice);
				_renderContext = new RenderContext
				{
					Batch = spriteBatch
				};
			}
		}

		public void Render()
		{
			var newBounds = BoundsFetcher();

			if (_bounds != newBounds)
			{
				InvalidateLayout();
			}

			_bounds = newBounds;

			if (_bounds.IsEmpty)
			{
				return;
			}

			UpdateInput();
			UpdateLayout();

			if (_scheduleMouseWheelFocus != null)
			{
				if (_scheduleMouseWheelFocus.AcceptsMouseWheelFocus)
				{
					_previousMouseWheelFocus = FocusedMouseWheelWidget;
					FocusedMouseWheelWidget = _scheduleMouseWheelFocus;
				}

				_scheduleMouseWheelFocus = null;
			}

			EnsureRenderContext();

			var oldScissorRectangle = CrossEngineStuff.GetScissor();

			_renderContext.Begin();

			CrossEngineStuff.SetScissor(_bounds);
			_renderContext.View = _bounds;
			_renderContext.Opacity = Opacity;

			if (Stylesheet.Current.DesktopStyle != null && 
				Stylesheet.Current.DesktopStyle.Background != null)
			{
				_renderContext.Draw(Stylesheet.Current.DesktopStyle.Background, _bounds);
			}

			foreach (var widget in ChildrenCopy)
			{
				if (widget.Visible)
				{
					widget.Render(_renderContext);
				}
			}

			_renderContext.End();

			CrossEngineStuff.SetScissor(oldScissorRectangle);
		}

		public void InvalidateLayout()
		{
			_layoutDirty = true;
		}

		public void UpdateLayout()
		{
			if (!_layoutDirty)
			{
				return;
			}

			// Find MenuBar
			MenuBar = null;

			foreach (var w in _widgets)
			{
				ProcessWidgets(w, widget =>
				{
					if (MenuBar == null && widget is HorizontalMenu)
					{
						MenuBar = (HorizontalMenu)widget;
					}
				});
			}

			// Update Active
			var activeWidget = GetActiveWidget(false);
			foreach (var w in _widgets)
			{
				var active = activeWidget == w;
				ProcessWidgets(w, widget =>
				{
					widget.Active = active;
				});
			}

			foreach (var widget in ChildrenCopy)
			{
				if (widget.Visible)
				{
					widget.Layout(_bounds);
				}
			}

			_layoutDirty = false;
		}

		public int CalculateTotalWidgets(bool visibleOnly)
		{
			var result = 0;
			foreach (var w in _widgets)
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

		private Widget GetActiveWidget(bool containsTouch = true)
		{
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var w = ChildrenCopy[i];
				if (w.Visible && w.Enabled &&
					((containsTouch && w.Bounds.Contains(TouchPosition)) ||
					!containsTouch))
				{
					return w;
				}
			}

			return null;
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

#if !XENKO
		private void UpdateTouch()
		{
			var touchState = TouchPanel.GetState();

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
			} else if (touchState.Count == 0 && _oldTouchState.Count > 0)
			{
				// Up
				IsTouchDown = false;
			}

			_oldTouchState = touchState;
		}
#endif

		public void UpdateInput()
		{
			if (MouseInfoGetter != null)
			{
				var mouseInfo = MouseInfoGetter();
				var mousePosition = mouseInfo.Position;

				if (SpriteBatchBeginParams.TransformMatrix != null)
				{
					// Apply transform
					var t = Vector2.Transform(
						new Vector2(mousePosition.X, mousePosition.Y),
						SpriteBatchBeginParams.InverseTransform);

					mousePosition = new Point((int)t.X, (int)t.Y);
				}

				MousePosition = mousePosition;

				HandleButton(mouseInfo.IsLeftButtonDown, _lastMouseInfo.IsLeftButtonDown, MouseButtons.Left);
				HandleButton(mouseInfo.IsMiddleButtonDown, _lastMouseInfo.IsMiddleButtonDown, MouseButtons.Middle);
				HandleButton(mouseInfo.IsRightButtonDown, _lastMouseInfo.IsRightButtonDown, MouseButtons.Right);
#if XENKO
				var handleWheel = mouseInfo.Wheel != 0;
#else
				var handleWheel = mouseInfo.Wheel != _lastMouseInfo.Wheel;
#endif

				if (handleWheel)
				{
					var delta = mouseInfo.Wheel;
#if !XENKO
					delta -= _lastMouseInfo.Wheel;
#endif
					MouseWheelChanged.Invoke(this, delta);

					if (FocusedMouseWheelWidget != null)
					{
						FocusedMouseWheelWidget.OnMouseWheel(delta);
					}
				}

				_lastMouseInfo = mouseInfo;
			}

			if (DownKeysGetter != null)
			{
				_downKeys = DownKeysGetter();

				if (_downKeys != null)
				{
					MyraEnvironment.ShowUnderscores = (MenuBar != null && MenuBar.OpenMenuItem != null) ||
													  _downKeys.Contains(Keys.LeftAlt) ||
													  _downKeys.Contains(Keys.RightAlt);

					if (_lastDownKeys != null)
					{
						foreach (var key in _downKeys)
						{
							if (!_lastDownKeys.Contains(key))
							{
								if (KeyDownHandler != null)
								{
									KeyDownHandler(key);
								}
							}
						}

						foreach (var key in _lastDownKeys)
						{
							if (!_downKeys.Contains(key))
							{
								// Key had been released
								KeyUp.Invoke(this, key);
								if (_focusedKeyboardWidget != null)
								{
									_focusedKeyboardWidget.OnKeyUp(key);
								}
							}
						}
					}
				}

				_lastDownKeys = _downKeys.ToArray();
			}

#if !XENKO
            try
            {
                UpdateTouch();
            }
            catch(Exception)
            {
            }
#endif
        }

		public void OnKeyDown(Keys key)
		{
			KeyDown.Invoke(this, key);

			if (MenuBar != null && MyraEnvironment.ShowUnderscores)
			{
				MenuBar.OnKeyDown(key);
			}
			else
			{
				// Small workaround: if key is escape  active widget is window
				// Send it there
				var asWindow = GetActiveWidget(false) as Window;
				if (asWindow != null && key == Keys.Escape && _focusedKeyboardWidget != asWindow)
				{
					asWindow.OnKeyDown(key);
				}

				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnKeyDown(key);

#if XENKO
					var ch = key.ToChar(_downKeys.Contains(Keys.LeftShift) ||
										_downKeys.Contains(Keys.RightShift));
					if (ch != null)
					{
						_focusedKeyboardWidget.OnChar(ch.Value);
					}
#endif
				}
			}

			if (key == Keys.Escape && ContextMenu != null)
			{
				HideContextMenu();
			}

#if MONOGAME
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

		public void OnChar(char c)
		{
			if (_focusedKeyboardWidget != null)
			{
				_focusedKeyboardWidget.OnChar(c);
			}

			Char.Invoke(this, c);
		}

		private void ProcessWidgets(Widget root, Action<Widget> operation)
		{
			if (!root.Visible)
			{
				return;
			}

			operation(root);

			var asContainer = root as Container;
			if (asContainer != null)
			{
				foreach (var w in asContainer.ChildrenCopy)
				{
					ProcessWidgets(w, operation);
				}
			}
		}

		private void UpdateWidgetsCopy()
		{
			if (!_widgetsDirty)
			{
				return;
			}

			_widgetsCopy.Clear();
			_widgetsCopy.AddRange(_widgets);

			_widgetsDirty = false;
		}

		private bool InternalIsPointOverGUI(Point p, Widget w)
		{
			if (!w.Visible || !w.ActualBounds.Contains(p))
			{
				return false;
			}

			// Non containers are completely solid
			var asContainer = w as Container;
			if (asContainer == null)
			{
				return true;
			}

			// Not real containers are solid as well
			if (!(w is Grid ||
				w is Panel ||
				w is SplitPane ||
				w is ScrollPane))
			{
				return true;
			}

			// Real containers are solid only if backround is set
			if (w.Background != null)
			{
				return true;
			}

			var asScrollPane = w as ScrollPane;
			if (asScrollPane != null)
			{
				// Special case
				if (asScrollPane._horizontalScrollingOn && asScrollPane._horizontalScrollbarFrame.Contains(p) ||
					asScrollPane._verticalScrollingOn && asScrollPane._verticalScrollbarFrame.Contains(p))
				{
					return true;
				}
			}

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
	}
}