namespace Myra.Graphics2D.UI.Styles
{
	public class ProgressBarStyle: ControlStyle
	{
		public IRenderable Filled { get; set; }

		public ProgressBarStyle()
		{
		}

		public ProgressBarStyle(ProgressBarStyle style): base(style)
		{
			Filled = style.Filled;
		}

		public override ControlStyle Clone()
		{
			return new ProgressBarStyle(this);
		}
	}
}
