using Myra.Utility;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Events;

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
#endif

namespace Myra.Graphics2D.UI
{
	partial class Widget: IInputEventsProcessor
	{
		private DateTime? _lastTouchDown;
		private DateTime? _lastMouseMovement;
		private Point _lastLocalTouchPosition;
		private Point? _localMousePosition;
		private Point? _localTouchPosition;

		[Browsable(false)]
		[XmlIgnore]
		public bool IsMouseInside => _localMousePosition != null;

		[Browsable(false)]
		[XmlIgnore]
		public Point? LocalMousePosition
		{
			get => _localMousePosition;
			private set
			{
				if (value == _localMousePosition)
				{
					return;
				}

				var oldValue = _localMousePosition;
				_localMousePosition = value;

				if (Desktop == null)
				{
					return;
				}

				if (value != null && oldValue == null)
				{
					InputEventsManager.Queue(this, InputEventType.MouseEntered);
				}
				else if (value == null && oldValue != null)
				{
					InputEventsManager.Queue(this, InputEventType.MouseLeft);
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.MouseMoved);
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsTouchInside => _localTouchPosition != null;

		[Browsable(false)]
		[XmlIgnore]
		public Point? LocalTouchPosition
		{
			get => _localTouchPosition;
			private set
			{
				if (value == _localTouchPosition)
				{
					return;
				}

				var oldValue = _localTouchPosition;
				_localTouchPosition = value;

				if (Desktop == null)
				{
					return;
				}

				if (value != null && oldValue == null)
				{
					if (Desktop.PreviousTouchPosition == null)
					{
						// Touch Down Event
						InputEventsManager.Queue(this, InputEventType.TouchDown);
						ProcessDoubleClick(value.Value);
					}
					else
					{
						// Touch Entered
						InputEventsManager.Queue(this, InputEventType.TouchEntered);
					}
				}
				else if (value == null && oldValue != null)
				{
					if (Desktop.TouchPosition == null)
					{
						InputEventsManager.Queue(this, InputEventType.TouchUp);
					}
					else
					{
						InputEventsManager.Queue(this, InputEventType.TouchLeft);
					}
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.TouchMoved);
				}
			}
		}

		protected internal virtual bool AcceptsMouseWheel => false;

		public event EventHandler PlacedChanged;
		public event EventHandler VisibleChanged;
		public event EventHandler EnabledChanged;

		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;
		public event EventHandler ArrangeUpdated;

		public event EventHandler MouseLeft;
		public event EventHandler MouseEntered;
		public event EventHandler MouseMoved;

		public event EventHandler TouchLeft;
		public event EventHandler TouchEntered;
		public event EventHandler TouchMoved;
		public event EventHandler TouchDown;
		public event EventHandler TouchUp;
		public event EventHandler TouchDoubleClick;

		public event EventHandler KeyboardFocusChanged;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;
		public event EventHandler<GenericEventArgs<char>> Char;

		private void ProcessDoubleClick(Point touchPos)
		{
			if (_lastTouchDown != null &&
				(DateTime.Now - _lastTouchDown.Value).TotalMilliseconds < MyraEnvironment.DoubleClickIntervalInMs &&
				Math.Abs(touchPos.X - _lastLocalTouchPosition.X) <= MyraEnvironment.DoubleClickRadius &&
				Math.Abs(touchPos.Y - _lastLocalTouchPosition.Y) <= MyraEnvironment.DoubleClickRadius)
			{
				_lastTouchDown = null;
				InputEventsManager.Queue(this, InputEventType.TouchDoubleClick);
			}
			else
			{
				_lastTouchDown = DateTime.Now;
				_lastLocalTouchPosition = LocalTouchPosition.Value;
			}
		}

		protected internal virtual void ProcessInput(InputContext inputContext)
		{
			if (!Visible || Desktop == null)
			{
				return;
			}

			if (!inputContext.MouseOrTouchHandled)
			{
				var oldContainsMouse = inputContext.ParentContainsMouse;
				var oldContainsTouch = inputContext.ParentContainsTouch;

				if (!Desktop.IsMobile)
				{
					if (inputContext.ParentContainsMouse)
					{
						if (ContainsGlobalPoint(Desktop.MousePosition))
						{
							LocalMousePosition = ToLocal(Desktop.MousePosition);
						}
						else
						{
							LocalMousePosition = null;
							inputContext.ParentContainsMouse = false;
						}
					}
					else
					{
						LocalMousePosition = null;
					}
				}

				if (Desktop.TouchPosition != null && inputContext.ParentContainsTouch)
				{
					if (ContainsGlobalPoint(Desktop.TouchPosition.Value))
					{
						LocalTouchPosition = ToLocal(Desktop.TouchPosition.Value);
					}
					else
					{
						LocalTouchPosition = null;
						inputContext.ParentContainsTouch = false;
					}
				}
				else
				{
					LocalTouchPosition = null;
				}

				if (IsMouseInside &&
					!Desktop.MouseWheelDelta.IsZero() &&
					AcceptsMouseWheel)
				{
					inputContext.MouseWheelWidget = this;
				}

				for (var i = _childrenCopy.Count - 1; i >= 0; i--)
				{
					var child = _childrenCopy[i];
					child.ProcessInput(inputContext);
				}

				if (IsModal)
				{
					// Modal widget prevents all further input processing
					inputContext.MouseOrTouchHandled = true;
				}
				else
				{
					if (!Desktop.IsMobile)
					{
						if (IsMouseInside && !InputFallsThrough(LocalMousePosition.Value))
						{
							inputContext.MouseOrTouchHandled = true;
						}
					}
					else
					{
						if (IsTouchInside && !InputFallsThrough(LocalTouchPosition.Value))
						{
							inputContext.MouseOrTouchHandled = true;
						}
					}
				}

				inputContext.ParentContainsMouse = oldContainsMouse;
				inputContext.ParentContainsTouch = oldContainsTouch;
			}
			else
			{
				if (!Desktop.IsMobile)
				{
					LocalMousePosition = null;
				}

				LocalTouchPosition = null;

				for (var i = _childrenCopy.Count - 1; i >= 0; i--)
				{
					var child = _childrenCopy[i];
					child.ProcessInput(inputContext);
				}
			}
		}

		void IInputEventsProcessor.ProcessEvent(InputEventType eventType)
		{
			// It's important to note that widget should process input events even if Desktop is null
			// Just add corresponding null checks in that case

			switch (eventType)
			{
				case InputEventType.MouseLeft:
					if (Desktop != null && Desktop.Tooltip != null && Desktop.Tooltip.Tag == this)
					{
						// Tooltip for this widget is shown
						Desktop.HideTooltip();
					}

					_lastMouseMovement = null;
					if (MouseCursor != null)
					{
						MyraEnvironment.MouseCursorType = MyraEnvironment.DefaultMouseCursorType;
					}

					OnMouseLeft();
					MouseLeft.Invoke(this);
					break;
				case InputEventType.MouseEntered:
					_lastMouseMovement = DateTime.Now;
					if (MouseCursor != null)
					{
						MyraEnvironment.MouseCursorType = MouseCursor.Value;
					}

					OnMouseEntered();
					MouseEntered.Invoke(this);
					break;
				case InputEventType.MouseMoved:
					_lastMouseMovement = DateTime.Now;

					OnMouseMoved();
					MouseMoved.Invoke(this);
					break;
				case InputEventType.MouseWheel:
					if (Desktop != null)
					{
						OnMouseWheel(Desktop.MouseWheelDelta);

						// Add yet another null check, since OnMouseWheel call might nullify the Desktop
						if (Desktop != null)
						{
							MouseWheelChanged.Invoke(this, Desktop.MouseWheelDelta);
						}
					}
					break;
				case InputEventType.TouchLeft:
					OnTouchLeft();
					TouchLeft.Invoke(this);
					break;
				case InputEventType.TouchEntered:
					OnTouchEntered();
					TouchEntered.Invoke(this);
					break;
				case InputEventType.TouchMoved:
					OnTouchMoved();
					TouchMoved.Invoke(this);
					break;
				case InputEventType.TouchDown:
					if (Desktop != null)
					{
						if (Enabled && AcceptsKeyboardFocus)
						{
							Desktop.FocusedKeyboardWidget = this;
						}

						if (DragHandle != null && DragHandle.IsTouchInside)
						{
							var parent = Parent != null ? (ITransformable)Parent : Desktop;
							_startPos = parent.ToLocal(new Vector2(Desktop.TouchPosition.Value.X, Desktop.TouchPosition.Value.Y));
							_startLeftTop = new Point(Left, Top);
						}
					}

					OnTouchDown();
					TouchDown.Invoke(this);
					break;
				case InputEventType.TouchUp:
					OnTouchUp();
					TouchUp.Invoke(this);
					break;
				case InputEventType.TouchDoubleClick:
					OnTouchDoubleClick();
					TouchDoubleClick.Invoke(this);
					break;
			}
		}

		public virtual void OnMouseLeft()
		{
		}

		public virtual void OnMouseEntered()
		{
		}

		public virtual void OnMouseMoved()
		{
		}

		public virtual void OnMouseWheel(float delta)
		{
		}

		public virtual void OnTouchLeft()
		{
		}

		public virtual void OnTouchEntered()
		{
		}

		public virtual void OnTouchMoved()
		{
		}

		public virtual void OnTouchDown()
		{
		}

		public virtual void OnTouchUp()
		{
		}

		public virtual void OnTouchDoubleClick()
		{
		}
	}
}
