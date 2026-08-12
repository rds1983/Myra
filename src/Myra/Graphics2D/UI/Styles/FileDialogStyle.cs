using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of file dialog widgets.
	/// </summary>
	public class FileDialogStyle : WindowStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the back navigation button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle BackButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the forward navigation button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle ForwardButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the parent folder navigation button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle ParentButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected files or folders.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of hovered files or folders.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the image displayed for folder items in the file list.
		/// </summary>
		public IImage IconFolder { get; set; }

		/// <summary>
		/// Gets or sets the image displayed for drive items in the file list.
		/// </summary>
		public IImage IconDrive { get; set; }
	}
}
