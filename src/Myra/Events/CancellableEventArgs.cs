using System;

namespace Myra.Events
{
	public class CancellableEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public CancellableEventArgs()
		{
		}
	}
}
