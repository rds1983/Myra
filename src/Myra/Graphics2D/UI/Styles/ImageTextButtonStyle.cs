using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of image-text button widgets (buttons with both icon and text).
	/// </summary>
	public class ImageTextButtonStyle : CheckButtonStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the button's text label.
		/// </summary>
		[Browsable(false)]
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageTextButtonStyle"/> class.
		/// </summary>
		public ImageTextButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageTextButtonStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source image-text button style to copy from.</param>
		public ImageTextButtonStyle(ImageTextButtonStyle style) : base(style)
		{
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this image-text button style.
		/// </summary>
		/// <returns>A new ImageTextButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}