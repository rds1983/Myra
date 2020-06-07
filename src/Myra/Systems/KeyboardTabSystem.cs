using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace Myra.Systems
{
    public class KeyboardTabSystem : BaseSystem
    {
        private KeyboardState _lastState;
        private readonly IList<Widget> _focusableWidgets = new List<Widget>();
        private int _focussedIndex = -1;
        private Widget _lastFocussedWidget;
        private Widget _currentFocussedWidget;

        public override void OnWidgetAddedToDesktop(Widget widget)
        {
            if (widget.AcceptsKeyboardFocus)
            {
                _focusableWidgets.Add(widget);
            }

            base.OnWidgetAddedToDesktop(widget);
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && _lastState.IsKeyUp(Keys.Tab))
            {
                FocusNextWidget();
            }

            _lastState = Keyboard.GetState();
        }

        private void FocusNextWidget()
        {
            if (_focusableWidgets.Count == 0) return;

            var startFocus = _focussedIndex;

            Widget nextFocusable;

            do
            {
                _focussedIndex++;
                ConstrainFocus();
                nextFocusable = _focusableWidgets[_focussedIndex];
            } while (_focussedIndex != startFocus && !CanFocusWidget(nextFocusable));

            if (CanFocusWidget(nextFocusable))
            {
                if (_currentFocussedWidget != null)
                {
                    _currentFocussedWidget.IsKeyboardFocused = false;
                }

                _lastFocussedWidget = _currentFocussedWidget;
                _currentFocussedWidget = nextFocusable;

                _currentFocussedWidget.IsKeyboardFocused = true;
                Desktop.FocusedKeyboardWidget = _currentFocussedWidget;
            }
        }

        private static bool CanFocusWidget(Widget focusable) =>
            focusable != null && focusable.Visible && focusable.Active &&
            focusable.Enabled;

        private void ConstrainFocus()
        {
            if (_focussedIndex >= _focusableWidgets.Count)
                _focussedIndex = 0;
            else if (_focussedIndex < 0)
                _focussedIndex = _focusableWidgets.Count - 1;
        }
    }
}