using System;

namespace Myra.Events
{
    public class TextDeletedEventArgs : MyraEventArgs
    {
        public int StartPosition
        {
            get;
        }

        public string Value
        {
            get;
        }

        public TextDeletedEventArgs(int startPosition, string value) : base(Graphics2D.UI.InputEventType.TextDeleted)
        {
            StartPosition = startPosition;
            Value = value;
        }
    }
}