namespace Myra.Graphics2D.UI.Styles
{
	public class FileDialogStyle : WindowStyle
	{
		public ImageButtonStyle BackButtonStyle { get; set; }
		public ImageButtonStyle ForwardButtonStyle { get; set; }
		public ImageButtonStyle ParentButtonStyle { get; set; }
		public IBrush SelectionBackground { get; set; }
		public IBrush SelectionHoverBackground { get; set; }
		public IImage IconFolder { get; set; }
		public IImage IconDrive { get; set; }
	}
}
