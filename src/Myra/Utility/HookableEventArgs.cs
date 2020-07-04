using System;
using System.Collections.Generic;
using System.Text;

namespace Myra.Utility
{
    public class HookableEventArgs : EventArgs
    {
        public bool WasHandled { get; private set; }

        private Action _hook;

        public void SetHandledFlag()
        {
            WasHandled = true;
        }

        public void AddHook(Action hook)
        {
            _hook = hook;
        }

        public bool InvokeHookIfNotHandled()
        {
            if (_hook == null || WasHandled)
            {
                return false;
            }

            _hook.Invoke();
            return true;
        }
    }
}
