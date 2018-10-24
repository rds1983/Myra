namespace Myra.Graphics2D.UI.Styles
{
	public class ProgressBarStyle: GridStyle
	{
		public Drawable Filled { get; set; }

		public ProgressBarStyle()
		{
		}

		public ProgressBarStyle(ProgressBarStyle style): base(style)
		{
			Filled = style.Filled;
		}

		public override WidgetStyle Clone()
		{
			return new ProgressBarStyle(this);
		}
	}
}
