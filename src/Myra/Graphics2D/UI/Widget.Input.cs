using Microsoft.Xna.Framework;
using Myra.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	partial class Widget
	{
		public const int DoubleClickIntervalInMs = 500;
		public const int DoubleClickRadius = 2;

		private DateTime? _lastTouchDown;
		private Point _lastLocalTouchPosition;
		private Point? _localMousePosition;
		private Point? _localTouchPosition;
		private readonly List<InputEventType> _scheduledInputEvents = new List<InputEventType>();

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

				if (value != null && oldValue == null)
				{
					if (MouseCursor != null)
					{
						MyraEnvironment.MouseCursorType = MouseCursor.Value;
					}
					ScheduleInputEvent(InputEventType.MouseEntered);
				}
				else if (value == null && oldValue != null)
				{
					if (MouseCursor != null)
					{
						MyraEnvironment.MouseCursorType = MyraEnvironment.DefaultMouseCursorType;
					}
					ScheduleInputEvent(InputEventType.MouseLeft);
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					ScheduleInputEvent(InputEventType.MouseMoved);
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

				if (value != null && oldValue == null)
				{
					if (Enabled && AcceptsKeyboardFocus)
					{
						Desktop.FocusedKeyboardWidget = this;
					}

					ScheduleInputEvent(InputEventType.TouchDown);
					ScheduleInputEvent(InputEventType.TouchEntered);
					ProcessDoubleClick(value.Value);

				}
				else if (value == null && oldValue != null)
				{
					ScheduleInputEvent(InputEventType.TouchUp);
					ScheduleInputEvent(InputEventType.TouchLeft);
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					ScheduleInputEvent(InputEventType.TouchMoved);
				}
			}
		}

		internal bool MouseEventFallsThrough
		{
			get
			{
				if (Desktop == null || !IsMouseInside)
				{
					return true;
				}

				return IsInputFallsThrough(Desktop.MousePosition);
			}
		}

		internal bool TouchEventFallsThrough
		{
			get
			{
				if (Desktop == null || !IsTouchInside)
				{
					return true;
				}

				return IsInputFallsThrough(Desktop.TouchPosition.Value);
			}
		}

		private void ProcessMouseInput(InputContext inputContext)
		{
			var isMouseInside = ContainsGlobalPoint(Desktop.MousePosition) && !inputContext.MouseOrTouchHandled;

			LocalMousePosition = isMouseInside ? ToLocal(Desktop.MousePosition) : (Point?)null;
		}

		private void ProcessTouchInput(InputContext inputContext)
		{
			var isTouchInside = Desktop.TouchPosition != null && ContainsGlobalPoint(Desktop.TouchPosition.Value) && !inputContext.MouseOrTouchHandled;
			LocalTouchPosition = isTouchInside ? ToLocal(Desktop.TouchPosition.Value) : (Point?)null;
		}

		private void ProcessDoubleClick(Point touchPos)
		{
			if (_lastTouchDown != null &&
				(DateTime.Now - _lastTouchDown.Value).TotalMilliseconds < DoubleClickIntervalInMs &&
				Math.Abs(touchPos.X - _lastLocalTouchPosition.X) <= DoubleClickRadius &&
				Math.Abs(touchPos.Y - _lastLocalTouchPosition.Y) <= DoubleClickRadius)
			{
				_lastTouchDown = null;
				ScheduleInputEvent(InputEventType.TouchDoubleClick);
			}
			else
			{
				_lastTouchDown = DateTime.Now;
			}
		}

		protected internal virtual void ProcessInput(InputContext inputContext)
		{
			ProcessMouseInput(inputContext);
			ProcessTouchInput(inputContext);

			if (IsMouseInside &&
				!inputContext.MouseOrTouchHandled &&
				!Desktop.MouseWheelDelta.IsZero())
			{
				ScheduleInputEvent(InputEventType.MouseWheel);
			}

			if (!MouseEventFallsThrough)
			{
				var b = MouseEventFallsThrough;

				inputContext.MouseOrTouchHandled = true;
			}

			foreach (var child in Children)
			{
				child.ProcessInput(inputContext);
			}
		}

		internal virtual bool IsInputFallsThrough(Point p)
		{
			return false;
		}

		internal void ProcessInputEvents()
		{
			foreach (var inputEvent in _scheduledInputEvents)
			{
				switch (inputEvent)
				{
					case InputEventType.MouseLeft:
						OnMouseLeft();
						MouseLeft.Invoke(this);
						break;
					case InputEventType.MouseEntered:
						OnMouseEntered();
						MouseEntered.Invoke(this);
						break;
					case InputEventType.MouseMoved:
						OnMouseMoved();
						MouseMoved.Invoke(this);
						break;
					case InputEventType.MouseWheel:
						OnMouseWheel(Desktop.MouseWheelDelta);
						MouseWheelChanged.Invoke(this, Desktop.MouseWheelDelta);
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
						if (DragHandle != null && DragHandle.IsTouchInside)
						{
							var parent = Parent != null ? (ITransformable)Parent : Desktop;
							_startPos = parent.ToLocal(new Vector2(Desktop.TouchPosition.Value.X, Desktop.TouchPosition.Value.Y));
							_startLeftTop = new Point(Left, Top);
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

			_scheduledInputEvents.Clear();

			foreach (var child in Children)
			{
				child.ProcessInputEvents();
			}
		}

		private void ScheduleInputEvent(InputEventType inputEvent)
		{
			_scheduledInputEvents.Add(inputEvent);
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
