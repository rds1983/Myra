using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of check button widgets.
	/// </summary>
	public class CheckButtonStyle : ImageButtonStyle
	{
		/// <summary>
		/// Gets or sets the spacing between the image and text in the check button.
		/// </summary>
		[DefaultValue(0)]
		public int ImageTextSpacing { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckButtonStyle"/> class.
		/// </summary>
		public CheckButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckButtonStyle"/> class as a deep copy of the specified style.
		/// </summary>
		/// <param name="style">The style to copy.</param>
		public CheckButtonStyle(CheckButtonStyle style) : base(style)
		{
			ImageTextSpacing = style.ImageTextSpacing;
		}

		/// <summary>
		/// Creates a deep copy of this button style.
		/// </summary>
		/// <returns>A new ButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new CheckButtonStyle(this);
		}
	}
}
