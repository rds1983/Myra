namespace Myra.Events
{
	/// <summary>
	/// Represents the method that handles Myra events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The event data.</param>
	public delegate void MyraEventHandler(object sender, MyraEventArgs e);

	/// <summary>
	/// Represents the method that handles Myra events with specific event argument types.
	/// </summary>
	/// <typeparam name="T">The type of event arguments.</typeparam>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The event data.</param>
	public delegate void MyraEventHandler<T>(object sender, T e) where T : MyraEventArgs;
}
