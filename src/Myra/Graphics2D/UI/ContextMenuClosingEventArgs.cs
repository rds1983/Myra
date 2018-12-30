using System;

namespace Myra.Graphics2D.UI
{
	public class ContextMenuClosingEventArgs : EventArgs
	{
		public Widget ContextMenu { get; private set; }
		public bool Cancel { get; set; }

		public ContextMenuClosingEventArgs(Widget contextMenu)
		{
			ContextMenu = contextMenu;
		}
	}
}