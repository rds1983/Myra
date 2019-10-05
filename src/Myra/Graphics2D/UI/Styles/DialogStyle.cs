namespace Myra.Graphics2D.UI.Styles
{
	public class DialogStyle: WindowStyle
	{
		/// <summary>
		/// "Ok" Button Style, if set to null then standard Button/Label styles are used
		/// </summary>
		public ImageTextButtonStyle OkButtonStyle
		{
			get; set;
		}

		/// <summary>
		/// "Cancel" Button Style, if set to null then standard Button/Label styles are used
		/// </summary>
		public ImageTextButtonStyle CancelButtonStyle
		{
			get; set;
		}

		public DialogStyle()
		{
		}

		public DialogStyle(DialogStyle style) : base(style)
		{
			OkButtonStyle = style.OkButtonStyle != null ? new ImageTextButtonStyle(style.OkButtonStyle) : null;
			CancelButtonStyle = style.CancelButtonStyle != null ? new ImageTextButtonStyle(style.CancelButtonStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new DialogStyle(this);
		}
	}
}
