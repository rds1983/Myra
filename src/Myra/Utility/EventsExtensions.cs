using System;

namespace Myra.Utility
{
	internal static class EventsExtensions
	{
		public static void Invoke(this EventHandler ev, object sender)
		{
			ev?.Invoke(sender, EventArgs.Empty);
		}

		public static void Invoke<T>(this EventHandler<GenericEventArgs<T>> ev, object sender, T data)
		{
			ev?.Invoke(sender, new GenericEventArgs<T>(data));
		}
	}
}