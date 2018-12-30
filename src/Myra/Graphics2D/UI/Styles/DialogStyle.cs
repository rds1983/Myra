namespace Myra.Graphics2D.UI.Styles
{
	public class DialogStyle: WindowStyle
	{
		public DialogStyle()
		{
		}

		public DialogStyle(DialogStyle style) : base(style)
		{
		}

		public override WidgetStyle Clone()
		{
			return new DialogStyle(this);
		}
	}
}
