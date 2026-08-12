namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of color picker dialog widgets.
	/// </summary>
	public class ColorPickerDialogStyle : WindowStyle
	{
		/// <summary>
		/// Gets or sets the checkerboard pattern image used to show transparency in the color preview.
		/// </summary>
		public IImage CheckerBoard { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected color items.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of hovered color items.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the color wheel image used in the hue selector.
		/// </summary>
		public IImage Wheel { get; set; }

		/// <summary>
		/// Gets or sets the gradient brush used in the saturation/value picker area.
		/// </summary>
		public IBrush Gradient { get; set; }

		/// <summary>
		/// Gets or sets the image used for the knob/crosshair in the saturation/value picker.
		/// </summary>
		public IImage VSPickerKnob { get; set; }
	}
}
