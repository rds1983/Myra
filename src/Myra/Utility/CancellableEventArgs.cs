using System;

namespace Myra.Utility
{
	public class CancellableEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public CancellableEventArgs()
		{
		}
	}
}
