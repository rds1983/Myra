using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of image button widgets.
	/// </summary>
	public class ImageButtonStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the button's image display.
		/// </summary>
		[Browsable(false)]
		public ImageStyle ImageStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageButtonStyle"/> class.
		/// </summary>
		public ImageButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageButtonStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source image button style to copy from.</param>
		public ImageButtonStyle(ImageButtonStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new ImageStyle(style.ImageStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this image button style.
		/// </summary>
		/// <returns>A new ImageButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ImageButtonStyle(this);
		}
	}
}
