using System;

namespace Myra.Graphics2D.UI
{
	public class ContextMenuClosingEventArgs : EventArgs
	{
		public Control ContextMenu { get; private set; }
		public bool Cancel { get; set; }

		public ContextMenuClosingEventArgs(Control contextMenu)
		{
			ContextMenu = contextMenu;
		}
	}
}