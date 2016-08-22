namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public Drawable Background { get; set; }
		public Drawable OverBackground { get; set; }
		public Drawable DisabledBackground { get; set; }
		public FrameInfo FrameInfo { get;set; }

		public WidgetStyle()
		{
			FrameInfo = new FrameInfo();
		}
	}
}
