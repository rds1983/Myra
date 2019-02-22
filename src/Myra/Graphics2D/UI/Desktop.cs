using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

		private RenderContext _renderContext;

		private bool _layoutDirty = true;
		private Rectangle _bounds;
		private bool _widgetsDirty = true;
		private Widget _focusedWidget;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();
		private readonly List<Widget> _focusableWidgets = new List<Widget>();
		private DateTime _lastTouch;
		private MouseInfo _lastMouseInfo;
		private IReadOnlyCollection<Keys> _downKeys, _lastDownKeys;

		public IReadOnlyCollection<Keys> DownKeys
		{
			get
			{
				return _downKeys;
			}
		}

		public MouseInfo LastMouseInfo
		{
			get
			{
				return _lastMouseInfo;
			}
		}

		private Point LastMousePosition
		{
			get
			{
				return _lastMouseInfo.Position;
			}
		}

		public Point MousePosition { get; private set; }
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

		public Rectangle Bounds
		{
			get { return _bounds; }

			set
			{
				if (value == _bounds)
				{
					return;
				}

				_bounds = value;
				InvalidateLayout();
			}
		}

		public Widget ContextMenu { get; private set; }

		public Widget FocusedWidget
		{
			get { return _focusedWidget; }

			set
			{
				if (value == _focusedWidget)
				{
					return;
				}

				if (_focusedWidget != null)
				{
					_focusedWidget.IterateFocusable(w => w.IsFocused = false);
				}

				_focusedWidget = value;

				if (_focusedWidget != null)
				{
					_focusedWidget.IterateFocusable(w => w.IsFocused = true);
				}
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

		public bool IsTouchDown
		{
			get; private set;
		}

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

		public event EventHandler<ContextMenuClosingEventArgs> ContextMenuClosing;
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		public Desktop()
		{
			Opacity = 1.0f;
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			MouseInfoGetter = DefaultMouseInfoGetter;
			DownKeysGetter = DefaultDownKeysGetter;

#if MONOGAME
			MyraEnvironment.Game.Window.TextInput += (s, a) =>
			{
				OnChar(a.Character);
			};
#elif FNA
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

		private void OnChar(char c)
		{
			var ev = Char;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<char>(c));
			}

			if (_focusedWidget != null)
			{
				_focusedWidget.IterateFocusable(w => w.OnChar(c));
			}
		}

		private void InputOnMouseDown()
		{
			if (ContextMenu != null && !ContextMenu.Bounds.Contains(MousePosition))
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

				ContextMenu.Left = position.X;
				ContextMenu.Top = position.Y;

				ContextMenu.Visible = true;

				_widgets.Add(ContextMenu);
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

			var ev = ContextMenuClosed;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<Widget>(ContextMenu));
			}

			ContextMenu = null;
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
			if (Bounds.IsEmpty)
			{
				return;
			}

			UpdateInput();
			UpdateLayout();

			EnsureRenderContext();

			var oldScissorRectangle = CrossEngineStuff.GetScissor();

			_renderContext.Begin();

			CrossEngineStuff.SetScissor(Bounds);
			_renderContext.View = Bounds;
			_renderContext.Opacity = Opacity;

			if (Stylesheet.Current.DesktopStyle != null && 
				Stylesheet.Current.DesktopStyle.Background != null)
			{
				_renderContext.Draw(Stylesheet.Current.DesktopStyle.Background, Bounds);
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

			ProcessWidgets();

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

		private Widget GetActiveWidget()
		{
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var w = ChildrenCopy[i];
				if (w.Visible && w.Enabled)
				{
					return w;
				}
			}

			return null;
		}

		public void HandleButton(bool isDown, bool wasDown, MouseButtons buttons)
		{
			var activeWidget = GetActiveWidget();
			if (activeWidget == null)
			{
				return;
			}

			if (isDown && !wasDown)
			{
				IsTouchDown = true;

				var ev = MouseDown;
				if (ev != null)
				{
					ev(this, new GenericEventArgs<MouseButtons>(buttons));
				}

				InputOnMouseDown();


				activeWidget.HandleMouseDown(buttons);

				var td = TouchDown;
				if (td != null)
				{
					td(this, EventArgs.Empty);
				}

				activeWidget.HandleTouchDown();

				if ((DateTime.Now - _lastTouch).TotalMilliseconds < DoubleClickIntervalInMs)
				{
					// Double click
					var ev2 = MouseDoubleClick;
					if (ev2 != null)
					{
						ev2(this, new GenericEventArgs<MouseButtons>(buttons));
					}

					activeWidget.HandleMouseDoubleClick(buttons);

					_lastTouch = DateTime.MinValue;
				}
				else
				{
					_lastTouch = DateTime.Now;
				}
			}
			else if (!isDown && wasDown)
			{
				IsTouchDown = false;

				var ev = MouseUp;
				if (ev != null)
				{
					ev(this, new GenericEventArgs<MouseButtons>(buttons));
				}

				activeWidget.HandleMouseUp(buttons);

				var tu = TouchUp;
				if (tu != null)
				{
					tu(this, EventArgs.Empty);
				}

				activeWidget.HandleTouchUp();
			}
		}

		public void UpdateInput()
		{
			if (MouseInfoGetter != null)
			{
				var mouseInfo = MouseInfoGetter();
				MousePosition = mouseInfo.Position;

				if (SpriteBatchBeginParams.TransformMatrix != null)
				{
					// Apply transform
					var t = Vector2.Transform(
						new Vector2(MousePosition.X, MousePosition.Y),
						SpriteBatchBeginParams.InverseTransform);

					MousePosition = new Point((int)t.X, (int)t.Y);
				}

				if (MousePosition.X != LastMousePosition.X || 
					MousePosition.Y != LastMousePosition.Y)
				{
					var ev = MouseMoved;
					if (ev != null)
					{
						ev(this, EventArgs.Empty);
					}

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
				}

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

					var ev = MouseWheelChanged;
					if (ev != null)
					{
						ev(null, new GenericEventArgs<float>(delta));
					}

					if (_focusedWidget != null)
					{
						_focusedWidget.IterateFocusable(w => w.OnMouseWheel(delta));
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
								var ev = KeyDown;
								if (ev != null)
								{
									ev(this, new GenericEventArgs<Keys>(key));
								}

								if (MenuBar != null && MyraEnvironment.ShowUnderscores)
								{
									MenuBar.OnKeyDown(key);
								}
								else
								{
									if (_focusedWidget != null)
									{
										_focusedWidget.IterateFocusable(w => w.OnKeyDown(key));

#if XENKO
										var ch = key.ToChar(_downKeys.Contains(Keys.LeftShift) ||
															_downKeys.Contains(Keys.RightShift));
										if (ch != null)
										{
											_focusedWidget.IterateFocusable(w => w.OnChar(ch.Value));
										}
#endif
									}
								}

								if (key == Keys.Escape && ContextMenu != null)
								{
									HideContextMenu();
								}
							}
						}

						foreach (var key in _lastDownKeys)
						{
							if (!_downKeys.Contains(key))
							{
								// Key had been released
								var ev = KeyUp;
								if (ev != null)
								{
									ev(this, new GenericEventArgs<Keys>(key));
								}

								if (_focusedWidget != null)
								{
									_focusedWidget.IterateFocusable(w => w.OnKeyUp(key));
								}
							}
						}
					}
				}

				_lastDownKeys = _downKeys.ToArray();
			}
		}

		internal void AddFocusableWidget(Widget w)
		{
			w.MouseDown += FocusableWidgetOnMouseDown;
			_focusableWidgets.Add(w);
		}

		internal void RemoveFocusableWidget(Widget w)
		{
			w.MouseDown -= FocusableWidgetOnMouseDown;
			_focusableWidgets.Remove(w);
		}

		private void ProcessWidgets(IEnumerable<Widget> widgets)
		{
			foreach (var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}


				if (MenuBar == null && w is HorizontalMenu)
				{
					MenuBar = (HorizontalMenu)w;
				}

				var asContainer = w as Container;
				if (asContainer != null)
				{
					ProcessWidgets(asContainer.ChildrenCopy);
				}
			}
		}

		private void ProcessWidgets()
		{
			MenuBar = null;

			ProcessWidgets(_widgets);
		}

		private void FocusableWidgetOnMouseDown(object sender, GenericEventArgs<MouseButtons> genericEventArgs)
		{
			var widget = (Widget)sender;

			if (!widget.IsFocused)
			{
				FocusedWidget = widget;
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
				if (asScrollPane._horizontalScrollbarVisible && asScrollPane._horizontalScrollbarFrame.Contains(p) ||
					asScrollPane._verticalScrollbarVisible && asScrollPane._verticalScrollbarFrame.Contains(p))
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