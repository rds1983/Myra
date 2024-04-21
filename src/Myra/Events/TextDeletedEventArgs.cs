using System;

namespace Myra.Events
{
    public class TextDeletedEventArgs : EventArgs
    {
        public int StartPosition
        {
            get;
        }

        public string Value
        {
            get;
        }
        
        public TextDeletedEventArgs(int startPosition, string value)
        {
            StartPosition = startPosition;
            Value = value;
        }
    }
}