namespace Myra.Graphics2D.UI.Styles
{
	public class ProgressBarStyle: WidgetStyle
	{
		public IBrush Filler { get; set; }

		public ProgressBarStyle()
		{
		}

		public ProgressBarStyle(ProgressBarStyle style): base(style)
		{
			Filler = style.Filler;
		}

		public override WidgetStyle Clone()
		{
			return new ProgressBarStyle(this);
		}
	}
}
