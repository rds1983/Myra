namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Defines the style of a button.
	/// </summary>
	public class ButtonStyle: WidgetStyle
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonStyle"/> class.
		/// </summary>
		public ButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source button style to copy from.</param>
		public ButtonStyle(ButtonStyle style): base(style)
		{
		}

		/// <summary>
		/// Creates a deep copy of this style.
		/// </summary>
		/// <returns>A new ButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone() => new ButtonStyle(this);
	}
}
