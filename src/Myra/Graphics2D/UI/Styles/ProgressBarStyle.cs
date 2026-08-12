namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of progress bar widgets.
	/// </summary>
	public class ProgressBarStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the brush used to fill the progress bar based on its current value.
		/// </summary>
		public IBrush Filler { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressBarStyle"/> class.
		/// </summary>
		public ProgressBarStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressBarStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source progress bar style to copy from.</param>
		public ProgressBarStyle(ProgressBarStyle style) : base(style)
		{
			Filler = style.Filler;
		}

		/// <summary>
		/// Creates a deep copy of this progress bar style.
		/// </summary>
		/// <returns>A new ProgressBarStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ProgressBarStyle(this);
		}
	}
}
