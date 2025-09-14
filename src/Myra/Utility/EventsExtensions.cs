using System;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace Myra.Utility
{
	internal static class EventsExtensions
	{
		public static void Invoke(this MyraEventHandler ev, InputEventType eventType)
		{
            ev?.Invoke(null, new MyraEventArgs(eventType));
		}

		public static void Invoke(this MyraEventHandler ev, object sender, InputEventType eventType)
		{
			ev?.Invoke(sender, new MyraEventArgs(eventType));
		}

		public static void Invoke<T>(this MyraEventHandler<GenericEventArgs<T>> ev, T data, InputEventType eventType)
		{
			ev?.Invoke(null, new GenericEventArgs<T>(data,eventType));
		}

		public static void Invoke<T>(this MyraEventHandler<GenericEventArgs<T>> ev, object sender, T data, InputEventType eventType)
		{
			ev?.Invoke(sender, new GenericEventArgs<T>(data, eventType));
		}
	}
}