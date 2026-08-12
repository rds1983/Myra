using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of window widgets.
	/// </summary>
	public class WindowStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the window's title bar text.
		/// </summary>
		[Browsable(false)]
		public LabelStyle TitleStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the window's close button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle CloseButtonStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowStyle"/> class.
		/// </summary>
		public WindowStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source window style to copy from.</param>
		public WindowStyle(WindowStyle style) : base(style)
		{
			TitleStyle = style.TitleStyle != null ? new LabelStyle(style.TitleStyle) : null;
			CloseButtonStyle = style.CloseButtonStyle != null ? new ImageButtonStyle(style.CloseButtonStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this window style.
		/// </summary>
		/// <returns>A new WindowStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new WindowStyle(this);
		}
	}
}
