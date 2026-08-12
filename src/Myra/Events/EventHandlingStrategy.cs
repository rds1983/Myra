namespace Myra.Events
{
	/// <summary>
	/// Specifies how input events are propagated through the widget hierarchy.
	/// </summary>
	public enum EventHandlingStrategy
	{
		/// <summary>Events are captured at the top level and propagate down to child widgets.</summary>
		EventCapturing,

		/// <summary>Events start at the widget level and bubble up to parent widgets.</summary>
		EventBubbling
	}
}
