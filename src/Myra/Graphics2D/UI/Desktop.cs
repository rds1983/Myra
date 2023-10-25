using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

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

		public IBrush Background { get; set; }

		public event EventHandler<CancellableEventArgs<Widget>> ContextMenuClosing;
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		public event EventHandler<CancellableEventArgs<Widget>> WidgetLosingKeyboardFocus;
		public event EventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		public Desktop()
		{
			Opacity = 1.0f;
			Widgets.CollectionChanged += WidgetsOnCollectionChanged;

			MouseInfoGetter = DefaultMouseInfoGetter;
			DownKeysGetter = DefaultDownKeysGetter;

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
			if (ContextMenu == null || ContextMenu.IsTouchInside)
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
			_inputContext.GlobalBounds = InternalBounds;
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var widget = ChildrenCopy[i];
				widget.ProcessInput(_inputContext);
			}

			// Only one widget at a time can receive mouse wheel event
			// So scheduling it here
			if (_inputContext.MouseWheelWidget != null)
			{
				_inputContext.MouseWheelWidget.ScheduleInputEvent(InputEventType.MouseWheel);
			}

			// Second input run: process input events
			ProcessInputEvents();
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var widget = ChildrenCopy[i];
				widget.ProcessInputEvents();
			}

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
			for (var i = ChildrenCopy.Count - 1; i >= 0; --i)
			{
				var w = ChildrenCopy[i];
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

				UpdateRecursiveLayout(i.ChildrenCopy);
			}
		}

		private Widget GetWidgetBy(Widget root, Func<Widget, bool> filter)
		{
			if (filter(root))
			{
				return root;
			}

			foreach (var w in root.ChildrenCopy)
			{
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

		private bool InternalIsPointOverGUI(Point p, Widget w)
		{
/*			if (!w.Visible || !w.ContainsGlobalPoint(p))
			{
				return false;
			}


			var localPos = w.ToLocal(p);
			if (!w.IsInputFallsThrough(localPos))
			{
				return true;
			}

			// Or if any child is solid
			foreach (var ch in w.ChildrenCopy)
			{
				if (InternalIsPointOverGUI(p, ch))
				{
					return true;
				}
			}*/

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