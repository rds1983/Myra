using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Myra.Samples.Inspector
{
    public class Input
    {
        public int SelectedIndex => selectedIndex;
        public int TextInfoIndex => infoIndex;
        
        private int selectedIndex, infoIndex;
        private bool lastDown, lastUp, lastLeft, lastRight;

        private readonly Func<int> _maxSelectionsGetter;
        private readonly Func<int> _maxInfosGetter;

        public Action<int> OnSelectionChanged;
        public Action<int> OnTextInfoChanged;
        
        public Input(Func<int> maxSelectionsGetter, Func<int> maxInfosGetter)
        {
            _maxSelectionsGetter = maxSelectionsGetter;
            _maxInfosGetter = maxInfosGetter;
        }
        
        public void Update(KeyboardState state)
        {
            // Prevent spam cycling by only triggering an input change once.
            bool thisDown = state[Keys.Down] == KeyState.Down;
            bool thisUp = state[Keys.Up] == KeyState.Down;
            bool thisLeft = state[Keys.Left] == KeyState.Down;
            bool thisRight = state[Keys.Right] == KeyState.Down;
			
            if (thisDown & !lastDown)
                CycleSelection(false);
            else if(thisUp & !lastUp)
                CycleSelection(true);
			
            if (thisLeft & !lastLeft)
                CycleInfo(false);
            else if(thisRight & !lastRight)
                CycleInfo(true);
			
            lastDown = thisDown;
            lastUp = thisUp;
            lastLeft = thisLeft;
            lastRight = thisRight;
        }
        
        private void CycleSelection(bool forward)
        {
            if (forward)
            {
                selectedIndex++;
                if (selectedIndex >= _maxSelectionsGetter.Invoke())
                    selectedIndex = 0;
            }
            else
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = _maxSelectionsGetter.Invoke() - 1;
            }
            OnSelectionChanged?.Invoke(selectedIndex); //widgets.Inspect( widgets.inspectables[selectedIndex] );
        }

        private void CycleInfo(bool forward)
        {
            if (forward)
            {
                infoIndex++;
                if (infoIndex >= _maxInfosGetter.Invoke())
                    infoIndex = 0;
            }
            else
            {
                infoIndex--;
                if (infoIndex < 0)
                    infoIndex = _maxInfosGetter.Invoke() - 1;
            }
            OnTextInfoChanged?.Invoke(infoIndex);
        }
    }
    
}