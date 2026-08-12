namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of spin button widgets.
	/// </summary>
	public class SpinButtonStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the spin button's up increment button.
		/// </summary>
		public ImageButtonStyle UpButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the spin button's down decrement button.
		/// </summary>
		public ImageButtonStyle DownButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the spin button's text input field.
		/// </summary>
		public TextBoxStyle TextBoxStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SpinButtonStyle"/> class.
		/// </summary>
		public SpinButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpinButtonStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source spin button style to copy from.</param>
		public SpinButtonStyle(SpinButtonStyle style) : base(style)
		{
			UpButtonStyle = style.UpButtonStyle != null ? new ImageButtonStyle(style.UpButtonStyle) : null;
			DownButtonStyle = style.DownButtonStyle != null ? new ImageButtonStyle(style.DownButtonStyle) : null;
			TextBoxStyle = style.TextBoxStyle != null ? new TextBoxStyle(style.TextBoxStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this spin button style.
		/// </summary>
		/// <returns>A new SpinButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SpinButtonStyle(this);
		}
	}
}