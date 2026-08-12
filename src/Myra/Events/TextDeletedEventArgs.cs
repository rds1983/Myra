namespace Myra.Events
{
	/// <summary>
	/// Provides data for text deletion events.
	/// </summary>
	public class TextDeletedEventArgs : MyraEventArgs
	{
		/// <summary>
		/// Gets the position where the text was deleted.
		/// </summary>
		public int StartPosition
		{
			get;
		}

		/// <summary>
		/// Gets the text that was deleted.
		/// </summary>
		public string Value
		{
			get;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextDeletedEventArgs"/> class.
		/// </summary>
		/// <param name="startPosition">The position where the text was deleted.</param>
		/// <param name="value">The text that was deleted.</param>
		public TextDeletedEventArgs(int startPosition, string value) : base(Graphics2D.UI.InputEventType.TextDeleted)
		{
			StartPosition = startPosition;
			Value = value;
		}
	}
}