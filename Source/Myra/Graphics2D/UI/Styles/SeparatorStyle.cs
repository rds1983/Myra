namespace Myra.Graphics2D.UI.Styles
{
	public class SeparatorStyle: WidgetStyle
	{
		public Drawable Image { get; set; }
		public int Thickness { get; set; }

		public SeparatorStyle()
		{
		}

		public SeparatorStyle(SeparatorStyle style): base(style)
		{
			Image = style.Image;
			Thickness = style.Thickness;
		}

		public override WidgetStyle Clone()
		{
			return new SeparatorStyle(this);
		}
	}
}
