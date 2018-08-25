using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSeparator : SeparatorWidget
	{
		public HorizontalSeparator(SeparatorStyle style) : base(Orientation.Horizontal, style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Center;
		}

		public HorizontalSeparator(string style)
			: this(Stylesheet.Current.HorizontalSeparatorStyles[style])
		{
		}

		public HorizontalSeparator()
			: this(Stylesheet.Current.HorizontalSeparatorStyle)
		{
		}
	}
}